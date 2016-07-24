using System;
using System.Collections;
using System.Reflection;

namespace Aqua.Reflection
{
    public static class ReflectionExtend
    {
        public static bool IsCanWrite(this MemberInfo member)
        {
            if (member is PropertyInfo)
                return ((PropertyInfo)member).CanWrite;
            return true;
        }
        public static bool IsCanRead(this MemberInfo member)
        {
            if (member is PropertyInfo)
                return ((PropertyInfo)member).CanRead;
            return true;
        }
        public static void SetMemberValue(this MemberInfo member, object obj, object value)
        {
            if (!IsCanWrite(member))
                return;
            if (member is PropertyInfo)
            {
                PropertyInfo pi = member as PropertyInfo;
                pi.SetValue(obj, value, null);
            }
            else if (member is FieldInfo)
            {
                FieldInfo fi = member as FieldInfo;
                fi.SetValue(obj, value);
            }
            return;
        }

        public static object GetMemberValue(this MemberInfo member, object obj)
        {
            if (!IsCanRead(member))
                return null;
            if (member is PropertyInfo)
            {
                PropertyInfo pi = member as PropertyInfo;
                return pi.GetValue(obj, null);
            }
            else if (member is FieldInfo)
            {
                FieldInfo fi = member as FieldInfo;
                return fi.GetValue(obj);
            }
            return null;
        }

        public static Type GetMemberType(this MemberInfo member)
        {
            Type type = null;
            if (member is PropertyInfo)
            {
                PropertyInfo pi = member as PropertyInfo;
                type = pi.PropertyType;
            }
            else if (member is FieldInfo)
            {
                FieldInfo fi = member as FieldInfo;
                type = fi.FieldType;
            }
            return type;
        }
        public static Type GetEnumerableType(this MemberInfo member)
        {
            Type type = null;
            if (member is PropertyInfo)
            {
                PropertyInfo pi = member as PropertyInfo;
                type = pi.PropertyType;
            }
            else if (member is FieldInfo)
            {
                FieldInfo fi = member as FieldInfo;
                type = fi.FieldType;
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (type.IsArray)
                {
                    type = type.GetElementType();
                }
                else if (type.IsGenericType)
                {
                    type = type.GetGenericArguments()[0];
                }
            }
            return type;
        }

        public static T GetFirstAttribute<T>(this MemberInfo member) where T : Attribute
        {
            object[] atts = member.GetCustomAttributes(typeof(T), true);
            for (int i = 0; i < atts.Length; i++)
            {
                if (atts[i] is T)
                    return atts[i] as T;
            }
            return null;
        }

        public static object GetProp(this object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo fi = obj.GetType().GetField(name, flags);
            if (fi != null)
            {
                return fi.GetValue(obj);
            }
            PropertyInfo pi = obj.GetType().GetProperty(name, flags);
            if (pi != null && pi.CanRead)
            {
                return pi.GetValue(obj, null);
            }
            throw new NullReferenceException("无法找到域:" + obj.GetType().Name + "->" + name);
        }

        public static void SetProp(this object obj, string name, object value)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo fi = obj.GetType().GetField(name, flags);
            if (fi != null)
            {
                fi.SetValue(obj, value);
                return;
            }
            PropertyInfo pi = obj.GetType().GetProperty(name, flags);
            if (pi != null && pi.CanRead)
            {
                pi.SetValue(obj, value, null);
                return;
            }
            throw new NullReferenceException("无法找到域:" + obj.GetType().Name + "->" + name);
        }

        public static object InvokeMethod(this object obj, string name, params object[] parameters)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = parameters[i].GetType();
            }
            MethodInfo mi = obj.GetType().GetMethod(name, flags, null, types, null);
            if (mi != null)
            {
                return mi.Invoke(obj, parameters);
            }
            throw new NullReferenceException("无法调用方法:" + obj.GetType().Name + "->" + name);
        }

        public static object InvokeMethod(this object obj, string name, Type[] types, object[] parameters)
        {
            MethodInfo mi = obj.GetType().GetMethod(name, types);
            if (mi != null)
            {
                return mi.Invoke(obj, parameters);
            }
            throw new NullReferenceException("无法调用方法:" + obj.GetType().Name + "->" + name);
        }

        public static object InvokeStaticMethod(this Type type, string name, params object[] parameters)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo[] methods = type.GetMethods(flags);
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name == name)
                {
                    ParameterInfo[] parameterInfos = methods[i].GetParameters();
                    if (parameters.Length != parameterInfos.Length)
                        continue;
                    bool isConfirm = true;
                    for (int j = 0; j < parameters.Length; j++)
                    {
                        Type t = parameterInfos[j].ParameterType;
                        if (parameters[j] != null && t != parameters[j].GetType())
                        {
                            isConfirm = false;
                            break;
                        }
                    }
                    if (!isConfirm)
                        continue;
                    return methods[i].Invoke(null, parameters);
                }
            }
            throw new NullReferenceException("无法调用方法:" + type.Name + "->" + name);
        }
        public static object InvokeArrayObjectAt(this object obj, int index)
        {
            Array arr = obj as Array;
            return arr.GetValue(index);
        }
        public static object GetStaticProp(this Type type, string name)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo fi = type.GetField(name, flags);
            if (fi != null)
            {
                return fi.GetValue(null);
            }
            PropertyInfo pi = type.GetProperty(name, flags);
            if (pi != null && pi.CanRead)
            {
                return pi.GetValue(null, null);
            }
            throw new NullReferenceException("无法找到域:" + type.Name + "->" + name);
        }
        public static void SetStaticProp(this Type type, string name, object value)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo fi = type.GetField(name, flags);
            if (fi != null)
            {
                fi.SetValue(null, value);
                return;
            }
            PropertyInfo pi = type.GetType().GetProperty(name, flags);
            if (pi != null && pi.CanRead)
            {
                pi.SetValue(null, value, null);
                return;
            }
            throw new NullReferenceException("无法找到域:" + type.Name + "->" + name);
        }


        public static void Resize(ref Array array, Type elemenType, int newSize)
        {
            if (array == null)
            {
                array = Array.CreateInstance(elemenType, newSize);
            }
            else
            {
                var newArray = Array.CreateInstance(elemenType, newSize);
                Array.Copy(array, newArray, array.Length);
                array = newArray;
            }
        }

        //public static void ForeachType(this Assembly assembly, FindTypeHandle handle)
        //{
        //    Type[] types = assembly.GetTypes();
        //    for (int i = 0; i < types.Length; i++)
        //    {
        //        if (!handle(types[i]))
        //            return;
        //    }
        //}
    }
}