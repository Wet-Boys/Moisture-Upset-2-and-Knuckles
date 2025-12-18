using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoistureUpsetRemix.Common.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoistureUpsetRemix.Common.AssetManagement;

public static class AssetManager
{
    public static IAssetProvider? AssetProvider { get; private set; }
    
    public static List<AssetBundle> AssetBundles => AssetProvider!.AssetBundles;
    public static Dictionary<string, int> AssetIndices => AssetProvider!.AssetIndices;
    
    public static void SetAssetProvider(IAssetProvider assetProvider)
    {
        AssetProvider = assetProvider;

        var methods = CollectOnAssetsLoadedListeners();
        
        AssetProvider.PopulateAssets();
        
        foreach (var method in methods)
            method.Invoke(null, []);
    }

    private static MethodInfo[] CollectOnAssetsLoadedListeners()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assembly.FullName.StartsWith("MoistureUpsetRemix"));

        var types = new HashSet<Type>();
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypesSafe())
                types.Add(type);
        }

        var methods = new HashSet<MethodInfo>();
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<RunMethodOnAssetsLoadedAttribute>();
            if (attribute is null)
                continue;

            var method = type.GetMethod(attribute.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (method is null)
                continue;
            
            methods.Add(method);
        } 
        
        return methods.ToArray();
    }

    public static T? Load<T>(string assetName) where T : Object => AssetProvider?.Load<T>(assetName);
}