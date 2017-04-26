#if NET20 || NET30 || NET35

using System;
using Theraot.Core.System.Diagnostics.Contracts;

namespace Theraot.Core.System.Threading.Tasks
{
    public partial class Task<TResult> : Task
    {
        internal TResult InternalResult;

        static Task()
        {
            ContinuationConvertion = done => (Task<TResult>)done.Result;
        }

        public Task(Func<TResult> function)
            : base(function, null, null, default(CancellationToken), TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Func<TResult> function, CancellationToken cancellationToken)
            : base(function, null, null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Func<TResult> function, TaskCreationOptions creationOptions)
            : base(function, null, null, default(CancellationToken), creationOptions, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : base(function, null, null, cancellationToken, creationOptions, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        internal Task(Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : base(function, null, null, cancellationToken, creationOptions, InternalTaskOptions.None, scheduler)
        {
            // Empty
        }

        internal Task(Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : base(function, state, null, cancellationToken, creationOptions, InternalTaskOptions.None, scheduler)
        {
            // Empty
        }

        internal Task(Delegate function, object state, Task parent, CancellationToken cancellationToken, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions, TaskScheduler scheduler)
            : base(function, state, parent, cancellationToken, creationOptions, internalOptions, scheduler)
        {
            // Empty
        }

        public TResult Result
        {
            get
            {
                Wait();
                if (IsFaulted)
                {
                    throw Exception;
                }
                if (IsCanceled)
                {
                    throw new AggregateException(new TaskCanceledException(this));
                }
                return InternalResult;
            }
        }

        internal static Func<Task<Task>, Task<TResult>> ContinuationConvertion { get; private set; }

        internal override void InnerInvoke()
        {
            var action = Action as Func<TResult>;
            if (action != null)
            {
                InternalResult = action();
                return;
            }
            var withState = Action as Func<object, TResult>;
            if (withState != null)
            {
                InternalResult = withState(State);
                return;
            }
            Contract.Assert(false, "Invalid Action in Task");
        }
    }
}

#endif