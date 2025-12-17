using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Common.Logging;
using MoistureUpsetRemix.Skins;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoistureUpsetRemix.Utils;

internal class AssetProvider : IAssetProvider
{
    public List<AssetBundle> AssetBundles { get; } = [];
    public Dictionary<string, int> AssetIndices { get; } = new();
    
    public void PopulateAssets()
    {
        foreach (var assetBundlePath in Directory.EnumerateFiles(FsUtils.AssetBundlesDir, "*", SearchOption.AllDirectories))
            AddBundle(Path.GetFileName(assetBundlePath));
    }

    public void AddBundle(string bundleName)
    {
        var assetBundleLoc = Path.Combine(FsUtils.AssetBundlesDir, bundleName);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundleLoc);

        int index = AssetBundles.Count;
        AssetBundles.Add(assetBundle);

        foreach (var assetName in assetBundle.GetAllAssetNames())
        {
            var path = assetName.ToLowerInvariant();
            
            if (path.StartsWith("assets/"))
                path = path["assets/".Length..];

            AssetIndices[path] = index;
        }
    }

    public T? Load<T>(string assetName) where T : Object
    {
        try
        {
            assetName = assetName.ToLowerInvariant();
            if (assetName.StartsWith("assets/"))
                assetName = assetName["assets/".Length..];

            int index = AssetIndices[assetName];
            return AssetBundles[index].LoadAsset<T>($"assets/{assetName}");
        }
        catch (Exception e)
        {
            Log.Error($"Couldn't load asset [{assetName}] exception: {e}");
            return null;
        }
    }

    public SkinDefMoistureUpsetRemix[] LoadEnemySkins()
    {
        return AssetIndices.Where(kvp =>
        {
            if (!kvp.Key.StartsWith("MoistureUpsetRemix/Enemies", StringComparison.InvariantCultureIgnoreCase))
                return false;

            var assetName = kvp.Key.Split('/').Last();
            return assetName.StartsWith("skin") && assetName.EndsWith(".asset");
        }).Select(kvp => Load<SkinDefMoistureUpsetRemix>(kvp.Key))
        .Where(skin => skin)
        .Select(skin => skin!)
        .ToArray();
    }
}