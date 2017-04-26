﻿// Needed for Workaround

using System.Diagnostics;

namespace Theraot.Core.Theraot.Core
{
    public static class StopwatchExtensions
    {
        public static void Restart(this Stopwatch stopwatch)
        {
            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}