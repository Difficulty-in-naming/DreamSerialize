#if NET20 || NET30 || NET35 || NET40

namespace Theraot.Core.System.Runtime.CompilerServices
{
    public interface INotifyCompletion
    {
        void OnCompleted(Action continuation);
    }
}

#endif