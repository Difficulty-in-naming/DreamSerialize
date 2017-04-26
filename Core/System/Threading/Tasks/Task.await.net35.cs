#if NET20 || NET30 || NET35

using Theraot.Core.System.Runtime.CompilerServices;

namespace Theraot.Core.System.Threading.Tasks
{
    public partial class Task
    {
        /// <summary>
        /// Gets an awaiter used to await this <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <returns>
        /// An awaiter instance.
        /// </returns>
        public TaskAwaiter GetAwaiter()
        {
            return new TaskAwaiter(this);
        }

        /// <summary>
        /// Creates and configures an awaitable object for awaiting the specified task.
        /// </summary>
        /// <param name="continueOnCapturedContext">true to automatic marshal back to the original call site's current SynchronizationContext
        ///             or TaskScheduler; otherwise, false.</param>
        /// <returns>
        /// The instance to be awaited.
        /// </returns>
        public ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
        {
            return new ConfiguredTaskAwaitable(this, continueOnCapturedContext);
        }
    }

    public partial class Task<TResult>
    {
        /// <summary>
        /// Gets an awaiter used to await this <see cref="T:Theraot.Core.System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="TResult">Specifies the type of data returned by the task.</typeparam>
        /// <returns>
        /// An awaiter instance.
        /// </returns>
        public new TaskAwaiter<TResult> GetAwaiter()
        {
            return new TaskAwaiter<TResult>(this);
        }

        /// <summary>
        /// Creates and configures an awaitable object for awaiting the specified task.
        /// </summary>
        /// <param name="continueOnCapturedContext">true to automatic marshal back to the original call site's current SynchronizationContext
        ///             or TaskScheduler; otherwise, false.</param>
        /// <returns>
        /// The instance to be awaited.
        /// </returns>
        public new ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
        {
            return new ConfiguredTaskAwaitable<TResult>(this, continueOnCapturedContext);
        }
    }
}

#endif