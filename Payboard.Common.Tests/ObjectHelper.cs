namespace Payboard.Common.Tests
{
    public static class ObjectHelper
    {
        public static object GetValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
    }
}