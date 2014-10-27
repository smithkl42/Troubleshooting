using System.Diagnostics;

namespace Payboard.Common
{
    public static class StopwatchHelper
    {
        public static string SummarizeAndReset(this Stopwatch sw, string name)
        {
            var result = string.Format("Stopwatch summary for {0}: {1} total milliseconds", name, sw.ElapsedMilliseconds);
            sw.Reset();
            return result;
        }
    }
}