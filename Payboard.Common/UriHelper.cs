using System;
using System.Linq;

namespace Payboard.Common
{
    public static class UriHelper
    {
        /// <summary>
        /// Returns true if uri1 and uri2 are basically the same URL, e.g., http://www.payboard.com and http://payboard.com.
        /// </summary>
        public static bool SortOfMatches(this Uri uri1, Uri uri2)
        {
            var host1 = uri1.MainDomain();
            var host2 = uri2.MainDomain();
            var path1 = uri1.AbsolutePath;
            var path2 = uri2.AbsolutePath;
            return (host1 + path1).Equals(host2 + path2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string MainDomain(this Uri uri)
        {
            var parts = uri.Host.Split('.');
            if (parts.Length < 3 || parts[0].Length > 3)
            {
                return uri.Host;
            }
            return string.Join(".", parts.Skip(1));
        }
    }
}