using System;
using System.Reflection;
using UnityEngine;

namespace MoistureUpsetRemix.Editor.Utils
{
    internal static class MonobehaviourUtils
    {
        public static void InvokeAwake(this MonoBehaviour script) =>
            GetMethodInfo(script, "Awake")?.Invoke(script, Array.Empty<object>());

        public static void InvokeStart(this MonoBehaviour script) =>
            GetMethodInfo(script, "Start")?.Invoke(script, Array.Empty<object>());
        
        public static void InvokeUpdate(this MonoBehaviour script) =>
            GetMethodInfo(script, "Update")?.Invoke(script, Array.Empty<object>());

        public static void InvokeMethod(this object instance, string methodName, params object[] parameters)
        {
            var type = instance.GetType();
            
            var methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            methodInfo?.Invoke(instance, parameters);
        }
        
        private static MethodInfo GetMethodInfo(MonoBehaviour script, string methodName)
        {
            var type = script.GetType();
            return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }
    }
}