using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace MoistureUpsetRemix.Editor.Utils
{
    internal static class SerializeUtils
    {
        public static void SetValueDirect(this SerializedProperty property, object value)
        {
            if (property == null)
                throw new NullReferenceException("SerializedProperty is null");

            object obj = property.serializedObject.targetObject;
            var propertyPath = property.propertyPath;
            var flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var paths = propertyPath.Split('.');
            FieldInfo field = null;

            for (var i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                if (obj == null)
                    throw new NullReferenceException("Can't set a value on a null instance");

                var type = obj.GetType();
                if (path == "Array")
                {
                    path = paths[++i];
                    if (obj is not IEnumerable iter)
                        //Property path thinks this property was an enumerable, but isn't. property path can't be parsed
                        throw new ArgumentException("SerializedProperty.PropertyPath [" + propertyPath +
                                                    "] thinks that [" + paths[i - 2] + "] is Enumerable.");

                    var sind = path.Split('[', ']');
                    var index = -1;

                    if (sind == null || sind.Length < 2)
                        // the array string index is malformed. the property path can't be parsed
                        throw new FormatException("PropertyPath [" + propertyPath + "] is malformed");

                    if (!int.TryParse(sind[1], out index))
                        //the array string index in the property path couldn't be parsed,
                        throw new FormatException("PropertyPath [" + propertyPath + "] is malformed");

                    obj = iter.ElementAtOrDefault(index);
                    continue;
                }

                field = type.GetField(path, flag);
                if (field == null)
                    //field wasn't found
                    throw new MissingFieldException("The field [" + path + "] in [" + propertyPath +
                                                    "] could not be found");

                if (i < paths.Length - 1)
                    obj = field.GetValue(obj);
            }

            var valueType = value.GetType();
            if (!valueType.Is(field.FieldType))
                // can't set value into field, types are incompatible
                throw new InvalidCastException("Cannot cast [" + valueType + "] into Field type [" + field.FieldType +
                                               "]");

            field.SetValue(obj, value);
        }


        public static bool Is(this Type type, Type baseType)
        {
            if (type == null)
                return false;
            if (baseType == null)
                return false;

            return baseType.IsAssignableFrom(type);
        }

        public static bool Is<T>(this Type type)
        {
            if (type == null) return false;
            var baseType = typeof(T);

            return baseType.IsAssignableFrom(type);
        }

        public static object ElementAtOrDefault(this IEnumerable collection, int index)
        {
            var enumerator = collection.GetEnumerator();
            var j = 0;
            for (; enumerator.MoveNext(); j++)
            {
                if (j == index)
                    break;
            }

            var element = (j == index)
                ? enumerator.Current
                : null;

            if (enumerator is IDisposable disposable)
                disposable.Dispose();

            return element;
        }
    }
}