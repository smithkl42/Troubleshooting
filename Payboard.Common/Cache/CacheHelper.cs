namespace Payboard.Common.Cache
{
    public static class CacheHelper
    {
        public static string GetKey(params object[] keyItems)
        {
            return string.Join(".", keyItems);
        }
    }
}