using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoistureUpsetRemix.Common.Utils;

public static class AssemblyUtils
{
    extension(Assembly assembly)
    {
        public Type[] GetTypesSafe()
        {
            var types = new HashSet<Type>();
            
            try
            {
                foreach (var type in assembly.GetTypes())
                    types.Add(type);
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (var type in e.Types.Where(type => type != null))
                    types.Add(type);
            }
            
            return types.ToArray();
        }
    }
}