using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Payboard.Common
{
    public static class TypeHelper
    {
        /// <summary>
        /// Clone only an object's public, non-reference type properties. 
        /// This is helpful when you want to separate out an object from its graph.
        /// </summary>
        public static T ShallowClone<T>(this T obj) where T : class
        {
            if (obj == null) return null;
            var newObj = Activator.CreateInstance<T>();
            obj.ShallowCopyTo(newObj);
            return newObj;
        }

        /// <summary>
        /// Copy only an object's public, non-reference type properties to a separate instance. 
        /// This is helpful when you want to separate out an object from its graph.
        /// </summary>
        public static T ShallowCopyTo<T>(this T source, T target, params string[] exceptForNames) where T : class
        {
            if (source == null) return null;
            var fields = typeof(T).GetFields();
            foreach (var field in fields)
            {
                if (field.IsPublic &&
                    (field.FieldType.IsValueType || field.FieldType == typeof(string)) &&
                    !exceptForNames.Contains(field.Name))
                {
                    field.SetValue(target, field.GetValue(source));
                }
            }
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if ((property.CanRead && property.CanWrite) &&
                    (property.PropertyType.IsValueType || property.PropertyType == typeof(string)) &&
                    !exceptForNames.Contains(property.Name))
                {
                    property.SetValue(target, property.GetValue(source, null), null);
                }
            }
            return target;
        }

        /// <summary>
        /// Copies the value properties of an object to another object, except for those specified in the exceptFor expressions
        /// </summary>
        public static T ShallowCopyTo<T, TException>(
            this T source, T target,
            Expression<Func<T, TException>> exceptFor) where T : class
        {
            var exceptForName = exceptFor.GetPropertyName();
            return source.ShallowCopyTo(target, exceptForName);
        }

        /// <summary>
        /// Copies the value properties of an object to another object, except for those specified in the exceptFor expressions
        /// </summary>
        public static T ShallowCopyTo<T, TException1, TException2>(
            this T source, T target,
            Expression<Func<T, TException1>> exceptFor1,
            Expression<Func<T, TException2>> exceptFor2) where T : class
        {
            var exceptForName1 = exceptFor1.GetPropertyName();
            var exceptForName2 = exceptFor2.GetPropertyName();
            return source.ShallowCopyTo(target, exceptForName1, exceptForName2);
        }

        /// <summary>
        /// Copies the value properties of an object to another object, except for those specified in the exceptFor expressions
        /// </summary>
        public static T ShallowCopyTo<T, TException1, TException2, TException3>(
            this T source, T target,
            Expression<Func<T, TException1>> exceptFor1,
            Expression<Func<T, TException2>> exceptFor2,
            Expression<Func<T, TException3>> exceptFor3) where T : class
        {
            var exceptForName1 = exceptFor1.GetPropertyName();
            var exceptForName2 = exceptFor2.GetPropertyName();
            var exceptForName3 = exceptFor3.GetPropertyName();
            return source.ShallowCopyTo(target, exceptForName1, exceptForName2, exceptForName3);
        }

        public static string GetPropertyName<TObj, TProp>(this Expression<Func<TObj, TProp>> property)
        {
            const string errorMessage = "The lambda expression 'property' should point to a valid property.";

            // For some reason, if the property is referring to a reference type, its body will show up as a MemberExpression; 
            // but if it's a value type, it'll show up as a UnaryExpression. Huh.
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = property.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                    if (memberExpression == null)
                    {
                        throw new ArgumentException(errorMessage);
                    }
                }
                else
                {
                    throw new ArgumentException(errorMessage);
                }
            }
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException(errorMessage);
            }
            string propertyName = propertyInfo.Name;
            return propertyName;
        }

        /// <summary>
        /// Returns a type name that looks like "Dictionary&lt;String, Object&gt;" rather than "Dictionary~2"
        /// </summary>
        public static string GetFriendlyTypeName(this Type t)
        {
            var typeName = t.Name.StripStartingWith("`");
            var genericArgs = t.GetGenericArguments();
            if (genericArgs.Length > 0)
            {
                typeName += "<";
                foreach (var genericArg in genericArgs)
                {
                    typeName += genericArg.GetFriendlyTypeName() + ", ";
                }
                typeName = typeName.TrimEnd(',', ' ') + ">";
            }
            return typeName;
        }
    }
}