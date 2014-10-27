using System;

namespace Payboard.Common
{
    /// <summary>
    ///     Simple class to allow freezing time in unit tests
    /// </summary>
    public static class SystemTime
    {
        /// <summary>
        ///     Allows time functions to be overridden in unit tests
        /// </summary>
        public static Func<DateTime> Now = () => DateTime.Now;

        /// <summary>
        ///     Allows time functions to be overridden in unit tests
        /// </summary>
        public static Func<DateTime> UtcNow = () => DateTime.UtcNow;
    }
}