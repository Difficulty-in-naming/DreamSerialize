// Needed for NET40

using Theraot.Core.System;

namespace Theraot.Core.Theraot.Collections
{
    public interface IProxyObservable<T> : IObservable<T>, IObserver<T>
    {
        // Empty
    }
}