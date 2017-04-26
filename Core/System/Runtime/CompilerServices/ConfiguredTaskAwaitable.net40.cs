#if NET20 || NET30 || NET35 || NET40

using System.Security;
using Theraot.Core.System.Threading.Tasks;

namespace Theraot.Core.System.Runtime.CompilerServices
{
    /// <summary>
    /// Provides an awaitable object that allows for configured awaits on <see cref="T:Theraot.Core.System.Threading.Tasks.Task`1"/>.
    /// </summary>
    ///
    /// <remarks>
    /// This type is intended for compiler use only.
    /// </remarks>
    public struct ConfiguredTaskAwaitable<TResult>
    {
        /// <summary>
        /// The underlying awaitable on whose logic this awaitable relies.
        /// </summary>
        private readonly ConfiguredTaskAwaiter _configuredTaskAwaiter;

        /// <summary>
        /// Initializes the <see cref="T:Theraot.Core.System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1"/>.
        /// </summary>
        /// <param name="task">The awaitable <see cref="T:Theraot.Core.System.Threading.Tasks.Task`1"/>.</param><param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.
        ///             </param>
        internal ConfiguredTaskAwaitable(Task<TResult> task, bool continueOnCapturedContext)
        {
            _configuredTaskAwaiter = new ConfiguredTaskAwaiter(task, continueOnCapturedContext);
        }

        /// <summary>
        /// Gets an awaiter for this awaitable.
        /// </summary>
        ///
        /// <returns>
        /// The awaiter.
        /// </returns>
        public ConfiguredTaskAwaiter GetAwaiter()
        {
            return _configuredTaskAwaiter;
        }

        /// <summary>
        /// Provides an awaiter for a <see cref="T:Theraot.Core.System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1"/>.
        /// </summary>
        ///
        /// <remarks>
        /// This type is intended for compiler use only.
        /// </remarks>
        public struct ConfiguredTaskAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>
            /// The task being awaited.
            /// </summary>
            private readonly Task<TResult> _task;

            /// <summary>
            /// Whether to attempt marshaling back to the original context.
            /// </summary>
            private readonly bool _continueOnCapturedContext;

            /// <summary>
            /// Gets whether the task being awaited is completed.
            /// </summary>
            ///
            /// <remarks>
            /// This property is intended for compiler user rather than use directly in code.
            /// </remarks>
            /// <exception cref="T:System.NullReferenceException">The awaiter was not properly initialized.</exception>
            public bool IsCompleted
            {
                get { return _task.IsCompleted; }
            }

            /// <summary>
            /// Initializes the <see cref="T:Theraot.Core.System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1.ConfiguredTaskAwaiter"/>.
            /// </summary>
            /// <param name="task">The awaitable <see cref="T:Theraot.Core.System.Threading.Tasks.Task`1"/>.</param><param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.
            ///             </param>
            internal ConfiguredTaskAwaiter(Task<TResult> task, bool continueOnCapturedContext)
            {
                _task = task;
                _continueOnCapturedContext = continueOnCapturedContext;
            }

            /// <summary>
            /// Schedules the continuation onto the <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/> associated with this <see cref="T:Theraot.Core.System.Runtime.CompilerServices.TaskAwaiter"/>.
            /// </summary>
            /// <param name="continuation">The action to invoke when the await operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception><exception cref="T:System.NullReferenceException">The awaiter was not properly initialized.</exception>
            /// <remarks>
            /// This method is intended for compiler user rather than use directly in code.
            /// </remarks>
            public void OnCompleted(Action continuation)
            {
                TaskAwaiter.OnCompletedInternal(_task, continuation, _continueOnCapturedContext);
            }

            /// <summary>
            /// Schedules the continuation onto the <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/> associated with this <see cref="T:Theraot.Core.System.Runtime.CompilerServices.TaskAwaiter"/>.
            /// </summary>
            /// <param name="continuation">The action to invoke when the await operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception><exception cref="T:System.InvalidOperationException">The awaiter was not properly initialized.</exception>
            /// <remarks>
            /// This method is intended for compiler user rather than use directly in code.
            /// </remarks>
            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation)
            {
                TaskAwaiter.OnCompletedInternal(_task, continuation, true);
            }

            /// <summary>
            /// Ends the await on the completed <see cref="T:Theraot.Core.System.Threading.Tasks.Task`1"/>.
            /// </summary>
            ///
            /// <returns>
            /// The result of the completed <see cref="T:Theraot.Core.System.Threading.Tasks.Task`1"/>.
            /// </returns>
            /// <exception cref="T:System.NullReferenceException">The awaiter was not properly initialized.</exception><exception cref="T:System.InvalidOperationException">The task was not yet completed.</exception><exception cref="T:Theraot.Core.System.Threading.Tasks.TaskCanceledException">The task was canceled.</exception><exception cref="T:System.Exception">The task completed in a Faulted state.</exception>
            public TResult GetResult()
            {
                TaskAwaiter.ValidateEnd(_task);
                return _task.Result;
            }
        }
    }

    /// <summary>
    /// Provides an awaitable object that allows for configured awaits on <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/>.
    /// </summary>
    ///
    /// <remarks>
    /// This type is intended for compiler use only.
    /// </remarks>
    public struct ConfiguredTaskAwaitable
    {
        /// <summary>
        /// The task being awaited.
        /// </summary>
        private readonly ConfiguredTaskAwaiter _configuredTaskAwaiter;

        /// <summary>
        /// Initializes the <see cref="T:Theraot.Core.System.Runtime.CompilerServices.ConfiguredTaskAwaitable"/>.
        /// </summary>
        /// <param name="task">The awaitable <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/>.</param><param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.
        ///             </param>
        internal ConfiguredTaskAwaitable(Task task, bool continueOnCapturedContext)
        {
            _configuredTaskAwaiter = new ConfiguredTaskAwaiter(task, continueOnCapturedContext);
        }

        /// <summary>
        /// Gets an awaiter for this awaitable.
        /// </summary>
        ///
        /// <returns>
        /// The awaiter.
        /// </returns>
        public ConfiguredTaskAwaiter GetAwaiter()
        {
            return _configuredTaskAwaiter;
        }

        /// <summary>
        /// Provides an awaiter for a <see cref="T:Theraot.Core.System.Runtime.CompilerServices.ConfiguredTaskAwaitable"/>.
        /// </summary>
        ///
        /// <remarks>
        /// This type is intended for compiler use only.
        /// </remarks>
        public struct ConfiguredTaskAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>
            /// The task being awaited.
            /// </summary>
            private readonly Task _task;

            /// <summary>
            /// Whether to attempt marshaling back to the original context.
            /// </summary>
            private readonly bool _continueOnCapturedContext;

            /// <summary>
            /// Gets whether the task being awaited is completed.
            /// </summary>
            ///
            /// <remarks>
            /// This property is intended for compiler user rather than use directly in code.
            /// </remarks>
            /// <exception cref="T:System.NullReferenceException">The awaiter was not properly initialized.</exception>
            public bool IsCompleted
            {
                get { return _task.IsCompleted; }
            }

            /// <summary>
            /// Initializes the <see cref="T:Theraot.Core.System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter"/>.
            /// </summary>
            /// <param name="task">The <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/> to await.</param><param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured
            ///             when BeginAwait is called; otherwise, false.
            ///             </param>
            internal ConfiguredTaskAwaiter(Task task, bool continueOnCapturedContext)
            {
                _task = task;
                _continueOnCapturedContext = continueOnCapturedContext;
            }

            /// <summary>
            /// Schedules the continuation onto the <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/> associated with this <see cref="T:Theraot.Core.System.Runtime.CompilerServices.TaskAwaiter"/>.
            /// </summary>
            /// <param name="continuation">The action to invoke when the await operation completes.</param>
            /// <exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
            /// <exception cref="T:System.NullReferenceException">The awaiter was not properly initialized.</exception>
            /// <remarks>
            /// This method is intended for compiler user rather than use directly in code.
            /// </remarks>
            public void OnCompleted(Action continuation)
            {
                TaskAwaiter.OnCompletedInternal(_task, continuation, _continueOnCapturedContext);
            }

            /// <summary>
            /// Schedules the continuation onto the <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/> associated with this <see cref="T:Theraot.Core.System.Runtime.CompilerServices.TaskAwaiter"/>.
            /// </summary>
            /// <param name="continuation">The action to invoke when the await operation completes.</param>
            /// <exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
            /// <exception cref="T:System.InvalidOperationException">The awaiter was not properly initialized.</exception>
            /// <remarks>
            /// This method is intended for compiler user rather than use directly in code.
            /// </remarks>
            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation)
            {
                TaskAwaiter.OnCompletedInternal(_task, continuation, true);
            }

            /// <summary>
            /// Ends the await on the completed <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/>.
            /// </summary>
            ///
            /// <returns>
            /// The result of the completed <see cref="T:Theraot.Core.System.Threading.Tasks.Task`1"/>.
            /// </returns>
            /// <exception cref="T:System.NullReferenceException">The awaiter was not properly initialized.</exception><exception cref="T:System.InvalidOperationException">The task was not yet completed.</exception><exception cref="T:Theraot.Core.System.Threading.Tasks.TaskCanceledException">The task was canceled.</exception><exception cref="T:System.Exception">The task completed in a Faulted state.</exception>
            public void GetResult()
            {
                TaskAwaiter.ValidateEnd(_task);
            }
        }
    }
}

#endif