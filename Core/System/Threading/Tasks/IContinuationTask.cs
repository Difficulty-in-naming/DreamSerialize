#if NET20 || NET30 || NET35

namespace Theraot.Core.System.Threading.Tasks
{
    internal interface IContinuationTask
    {
        Task Antecedent { get; }
    }
}

#endif