#if NET20 || NET30 || NET35

using System;
using System.Threading;
using Theraot.Core.System.Diagnostics.Contracts;

namespace Theraot.Core.System.Threading.Tasks
{
    internal sealed class ContinuationResultTaskFromTask<TResult> : Task<TResult>, IContinuationTask
    {
        private Task _antecedent;

        public ContinuationResultTaskFromTask(Task antecedent, Delegate function, object state, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions)
            : base(function, state, InternalCurrentIfAttached(creationOptions), default(CancellationToken), creationOptions, internalOptions, antecedent.ExecutingTaskScheduler)
        {
            Contract.Requires(function is Func<Task, TResult> || function is Func<Task, object, TResult>, "Invalid delegate type in ContinuationResultTaskFromTask");
            _antecedent = antecedent;
            CapturedContext = ExecutionContext.Capture();
        }

        Task IContinuationTask.Antecedent
        {
            get { return _antecedent; }
        }

        /// <summary>
        /// Evaluates the value selector of the Task which is passed in as an object and stores the result.
        /// </summary>
        internal override void InnerInvoke()
        {
            // Get and null out the antecedent.  This is crucial to avoid a memory
            // leak with long chains of continuations.
            var antecedent = _antecedent;
            Contract.Assert(antecedent != null, "No antecedent was set for the ContinuationResultTaskFromTask.");
            _antecedent = null;
            // Invoke the delegate
            Contract.Assert(Action != null);
            var func = Action as Func<Task, TResult>;
            if (func != null)
            {
                InternalResult = func(antecedent);
                return;
            }
            var funcWithState = Action as Func<Task, object, TResult>;
            if (funcWithState != null)
            {
                InternalResult = funcWithState(antecedent, State);
                return;
            }
            Contract.Assert(false, "Invalid Action in ContinuationResultTaskFromTask");
        }
    }
}

#endif