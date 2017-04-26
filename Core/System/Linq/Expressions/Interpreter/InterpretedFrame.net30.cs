#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Theraot.Core.System.Runtime.CompilerServices;

namespace Theraot.Core.System.Linq.Expressions.Interpreter
{
    internal sealed class InterpretedFrame
    {
        [ThreadStatic]
        public static InterpretedFrame CurrentFrame;

        internal readonly Interpreter Interpreter;

        private readonly int[] _continuations;

        private int _continuationIndex;
        private int _pendingContinuation;
        private object _pendingValue;

        public readonly object[] Data;

        public readonly IStrongBox[] Closure;

        public int StackIndex;
        public int InstructionIndex;

#if FEATURE_THREAD_ABORT
        // When a ThreadAbortException is raised from interpreted code this is the first frame that caught it.
        // No handlers within this handler re-abort the current thread when left.
        public ExceptionHandler CurrentAbortHandler;
#endif

        internal InterpretedFrame(Interpreter interpreter, IStrongBox[] closure)
        {
            Interpreter = interpreter;
            StackIndex = interpreter.LocalCount;
            Data = new object[StackIndex + interpreter.Instructions.MaxStackDepth];

            var c = interpreter.Instructions.MaxContinuationDepth;
            if (c > 0)
            {
                _continuations = new int[c];
            }

            Closure = closure;

            _pendingContinuation = -1;
            _pendingValue = Interpreter.NoValue;
        }

        public DebugInfo GetDebugInfo(int instructionIndex)
        {
            return DebugInfo.GetMatchingDebugInfo(Interpreter.DebugInfos, instructionIndex);
        }

        public string Name
        {
            get { return Interpreter.Name; }
        }

        #region Data Stack Operations

        public void Push(object value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(bool value)
        {
            Data[StackIndex++] = value ? ScriptingRuntimeHelpers.True : ScriptingRuntimeHelpers.False;
        }

        public void Push(int value)
        {
            Data[StackIndex++] = ScriptingRuntimeHelpers.Int32ToObject(value);
        }

        public void Push(byte value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(sbyte value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(short value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(ushort value)
        {
            Data[StackIndex++] = value;
        }

        public object Pop()
        {
            return Data[--StackIndex];
        }

        internal void SetStackDepth(int depth)
        {
            StackIndex = Interpreter.LocalCount + depth;
        }

        public object Peek()
        {
            return Data[StackIndex - 1];
        }

        public void Dup()
        {
            var i = StackIndex;
            Data[i] = Data[i - 1];
            StackIndex = i + 1;
        }

        #endregion Data Stack Operations

        #region Stack Trace

        public InterpretedFrame Parent { get; set; }

        public static bool IsInterpretedFrame(MethodBase method)
        {
            //ContractUtils.RequiresNotNull(method, "method");
            return method.DeclaringType == typeof(Interpreter) && method.Name == "Run";
        }

        public IEnumerable<InterpretedFrameInfo> GetStackTraceDebugInfo()
        {
            var frame = this;
            do
            {
                yield return new InterpretedFrameInfo(frame.Name, frame.GetDebugInfo(frame.InstructionIndex));
                frame = frame.Parent;
            } while (frame != null);
        }

        internal void SaveTraceToException(Exception exception)
        {
            if (exception.Data[typeof(InterpretedFrameInfo)] == null)
            {
                exception.Data[typeof(InterpretedFrameInfo)] = new List<InterpretedFrameInfo>(GetStackTraceDebugInfo()).ToArray();
            }
        }

        public static InterpretedFrameInfo[] GetExceptionStackTrace(Exception exception)
        {
            return exception.Data[typeof(InterpretedFrameInfo)] as InterpretedFrameInfo[];
        }

#if DEBUG

        internal string[] Trace
        {
            get
            {
                var trace = new List<string>();
                var frame = this;
                do
                {
                    trace.Add(frame.Name);
                    frame = frame.Parent;
                } while (frame != null);
                return trace.ToArray();
            }
        }

#endif

        internal InterpretedFrame Enter()
        {
            var currentFrame = CurrentFrame;
            CurrentFrame = this;
            return Parent = currentFrame;
        }

        internal void Leave(InterpretedFrame prevFrame)
        {
            CurrentFrame = prevFrame;
        }

        #endregion Stack Trace

        #region Continuations

        internal bool IsJumpHappened()
        {
            return _pendingContinuation >= 0;
        }

        public void RemoveContinuation()
        {
            _continuationIndex--;
        }

        public void PushContinuation(int continuation)
        {
            _continuations[_continuationIndex++] = continuation;
        }

        public int YieldToCurrentContinuation()
        {
            var target = Interpreter.Labels[_continuations[_continuationIndex - 1]];
            SetStackDepth(target.StackDepth);
            return target.Index - InstructionIndex;
        }

        /// <summary>
        /// Get called from the LeaveFinallyInstruction
        /// </summary>
        public int YieldToPendingContinuation()
        {
            Debug.Assert(_pendingContinuation >= 0);
            var pendingTarget = Interpreter.Labels[_pendingContinuation];

            // the current continuation might have higher priority (continuationIndex is the depth of the current continuation):
            if (pendingTarget.ContinuationStackDepth < _continuationIndex)
            {
                var currentTarget = Interpreter.Labels[_continuations[_continuationIndex - 1]];
                SetStackDepth(currentTarget.StackDepth);
                return currentTarget.Index - InstructionIndex;
            }

            SetStackDepth(pendingTarget.StackDepth);
            if (_pendingValue != Interpreter.NoValue)
            {
                Data[StackIndex - 1] = _pendingValue;
            }

            // Set the _pendingContinuation and _pendingValue to the default values if we finally gets to the Goto target
            _pendingContinuation = -1;
            _pendingValue = Interpreter.NoValue;
            return pendingTarget.Index - InstructionIndex;
        }

        internal void PushPendingContinuation()
        {
            Push(_pendingContinuation);
            Push(_pendingValue);

            _pendingContinuation = -1;
            _pendingValue = Interpreter.NoValue;
        }

        internal void PopPendingContinuation()
        {
            _pendingValue = Pop();
            _pendingContinuation = (int)Pop();
        }

        private static MethodInfo _goto;
        private static MethodInfo _voidGoto;

        internal static MethodInfo GotoMethod
        {
            get { return _goto ?? (_goto = typeof(InterpretedFrame).GetMethod("Goto")); }
        }

        internal static MethodInfo VoidGotoMethod
        {
            get { return _voidGoto ?? (_voidGoto = typeof(InterpretedFrame).GetMethod("VoidGoto")); }
        }

        public int VoidGoto(int labelIndex)
        {
            return Goto(labelIndex, Interpreter.NoValue, /*gotoExceptionHandler*/ false);
        }

        public int Goto(int labelIndex, object value, bool gotoExceptionHandler)
        {
            // TODO: we know this at compile time (except for compiled loop):
            var target = Interpreter.Labels[labelIndex];
            Debug.Assert(!gotoExceptionHandler || (gotoExceptionHandler && _continuationIndex == target.ContinuationStackDepth),
                "When it's time to jump to the exception handler, all previous finally blocks should already be processed");

            if (_continuationIndex == target.ContinuationStackDepth)
            {
                SetStackDepth(target.StackDepth);
                if (value != Interpreter.NoValue)
                {
                    Data[StackIndex - 1] = value;
                }
                return target.Index - InstructionIndex;
            }

            // if we are in the middle of executing jump we forget the previous target and replace it by a new one:
            _pendingContinuation = labelIndex;
            _pendingValue = value;
            return YieldToCurrentContinuation();
        }

        #endregion Continuations
    }
}

#endif