using System.Collections.Generic;

namespace Payboard.Common
{
    public static class GenericHelpers
    {
        public static T OrIfGreater<T>(this T obj1, T obj2)
        {
            if (Comparer<T>.Default.Compare(obj2, obj1) > 0) return obj2;
            return obj1;
        }

        public static T OrIfLesser<T>(this T obj1, T obj2)
        {
            if (Comparer<T>.Default.Compare(obj2, obj1) < 0) return obj2;
            return obj1;
        }

        /// <summary>
        ///     This variant selects the lesser of two values, unless one of them is not null, in which case it selects the one
        ///     that is not null
        /// </summary>
        public static T? OrIfLesser<T>(this T? obj1, T? obj2) where T : struct
        {
            if (obj2 == null) return obj1;
            if (obj1 == null) return obj2;
            if (Comparer<T?>.Default.Compare(obj2, obj1) < 0) return obj2;
            return obj1;
        }
    }
}