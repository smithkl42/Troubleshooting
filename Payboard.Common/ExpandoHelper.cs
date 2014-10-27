using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Payboard.Common
{
    public static class ExpandoHelper
    {
        public static ExpandoObject ToExpando(this object obj, bool retainCase = false)
        {
            if (obj == null)
            {
                return null;
            }
            var exp = new ExpandoObject();
            var dict = exp as IDictionary<string, object>;
            var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in properties)
            {
                if (prop.IsSimple())
                {
                    var propName = retainCase ? prop.Name : prop.Name.ToLowerInvariant();
                    dict[propName] = prop.GetValue(obj);
                }
            }
            return exp;
        }

        public static bool IsSimple(this PropertyInfo prop)
        {
            return prop.PropertyType == typeof(int) ||
                   prop.PropertyType == typeof(double) ||
                   prop.PropertyType == typeof(decimal) ||
                   prop.PropertyType == typeof(bool) ||
                   prop.PropertyType == typeof(byte) ||
                   prop.PropertyType == typeof(int?) ||
                   prop.PropertyType == typeof(double?) ||
                   prop.PropertyType == typeof(decimal?) ||
                   prop.PropertyType == typeof(bool?) ||
                   prop.PropertyType == typeof(byte?) ||
                   prop.PropertyType == typeof(string) ||
                   prop.PropertyType == typeof(Guid);
        }

        public static ExpandoObject ToFullExpando(this object obj, bool retainCase = false)
        {
            var exp = new ExpandoObject();
            var dict = exp as IDictionary<string, object>;
            var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in properties)
            {
                var propName = retainCase ? prop.Name : prop.Name.ToLowerInvariant();
                dict[propName] = prop.GetValue(obj);
            }
            return exp;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            var target = new Dictionary<TKey, TValue>();
            foreach (var value in source)
            {
                target.Add(value.Key, value.Value);
            }
            return target;
        }
    }
}