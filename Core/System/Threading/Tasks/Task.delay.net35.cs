#if NET20 || NET30 || NET35

using System;
using Theraot.Core.System.Diagnostics.Contracts;

namespace Theraot.Core.System.Threading.Tasks
{
    public partial class Task
    {
        /// <summary>A task that's already been completed successfully.</summary>
        private static Task _completedTask;

        /// <summary>Gets a task that's already been completed successfully.</summary>
        /// <remarks>May not always return the same instance.</remarks>
        internal static Task CompletedTask
        {
            get
            {
                var completedTask = _completedTask;
                if (completedTask == null)
                {
                    _completedTask = completedTask = CreateCompletedTask();
                }
                return completedTask;
            }
        }

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="delay"/> is less than -1 or greater than Int32.MaxValue.
        /// </exception>
        /// <remarks>
        /// After the specified time delay, the Task is completed in RanToCompletion state.
        /// </remarks>
        public static Task Delay(TimeSpan delay)
        {
            return Delay(delay, default(CancellationToken));
        }

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned Task</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="delay"/> is less than -1 or greater than Int32.MaxValue.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The provided <paramref name="cancellationToken"/> has already been disposed.
        /// </exception>
        /// <remarks>
        /// If the cancellation token is signaled before the specified time delay, then the Task is completed in
        /// Canceled state.  Otherwise, the Task is completed in RanToCompletion state once the specified time
        /// delay has expired.
        /// </remarks>
        public static Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        {
            var totalMilliseconds = (long)delay.TotalMilliseconds;
            if (totalMilliseconds < -1 || totalMilliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("delay", "The value needs to translate in milliseconds to - 1(signifying an infinite timeout), 0 or a positive integer less than or equal to Int32.MaxValue");
            }

            return Delay((int)totalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="millisecondsDelay"/> is less than -1.
        /// </exception>
        /// <remarks>
        /// After the specified time delay, the Task is completed in RanToCompletion state.
        /// </remarks>
        public static Task Delay(int millisecondsDelay)
        {
            return Delay(millisecondsDelay, default(CancellationToken));
        }

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned Task</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="millisecondsDelay"/> is less than -1.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The provided <paramref name="cancellationToken"/> has already been disposed.
        /// </exception>
        /// <remarks>
        /// If the cancellation token is signaled before the specified time delay, then the Task is completed in
        /// Canceled state.  Otherwise, the Task is completed in RanToCompletion state once the specified time
        /// delay has expired.
        /// </remarks>
        public static Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
        {
            // Throw on non-sensical time
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay", "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.");
            }
            Contract.EndContractBlock();
            // some short-cuts in case quick completion is in order
            if (cancellationToken.IsCancellationRequested)
            {
                // return a Task created as already-Canceled
                return FromCancellation(cancellationToken);
            }
            if (millisecondsDelay == 0)
            {
                // return a Task created as already-RanToCompletion
                return CompletedTask;
            }
            var source = new TaskCompletionSource<bool>();
            if (millisecondsDelay > 0)
            {
                var timeout =
                    new global::Theraot.Core.Theraot.Threading.Timeout
                    (
                        () =>
                        {
                            try
                            {
                                source.SetResult(true);
                            }
                            catch (InvalidOperationException exception)
                            {
                                // Already cancelled
                                GC.KeepAlive(exception);
                            }
                        },
                        millisecondsDelay,
                        cancellationToken
                    );
                source.Task.SetPromiseCheck(() => timeout.CheckRemaining());
            }
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register
                    (
                        () =>
                        {
                            try
                            {
                                source.SetCanceled();
                            }
                            catch (InvalidOperationException exception)
                            {
                                // Already timeout
                                GC.KeepAlive(exception);
                            }
                        }
                    );
            }
            return source.Task;
        }
    }
}

#endif