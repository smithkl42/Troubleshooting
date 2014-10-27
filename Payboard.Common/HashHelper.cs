using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payboard.Common
{
    public static class HashHelper
    {
        public static int GetIntHash(this int u)
        {
            // Randomize the hash value further by multiplying by a large prime,
            // then adding the hi and lo DWORDs of the 64-bit result together. This
            // produces an excellent randomization very efficiently.

            long l = u * 0x7ff19519;  // this number is prime.
            return (int)(l + (l >> 32));
        }

        public static double GetDoubleHash(this int u)
        {
            var hash = u.GetIntHash();
            return hash / (double)int.MaxValue;
        }

        public static int GetBucket(this int u, int size)
        {
            var hash = u.GetIntHash();
            return Math.Abs(hash % size);
        }

        public static int GetBucket(this Guid g, int size)
        {
            var hash = g.GetHashCode();
            return Math.Abs(hash % size);
        }
    }
}
