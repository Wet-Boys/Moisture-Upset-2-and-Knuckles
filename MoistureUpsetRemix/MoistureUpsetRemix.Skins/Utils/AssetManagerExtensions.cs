using System;
using System.Collections.Generic;
using System.Linq;
using MoistureUpsetRemix.Common.AssetManagement;

namespace MoistureUpsetRemix.Skins.Utils;

[RunMethodOnAssetsLoaded(nameof(OnAssetsLoaded))]
public static class AssetManagerExtensions
{
    private static Dictionary<string, string> _skinToPathLutDict = new();

    private static void OnAssetsLoaded()
    {
        _skinToPathLutDict = AssetManager.AssetIndices.Where(kvp =>
        {
            if (!kvp.Key.StartsWith("MoistureUpsetRemix/Enemies", StringComparison.InvariantCultureIgnoreCase))
                return false;

            var assetName = kvp.Key.Split('/').Last();
            return assetName.StartsWith("skin") && assetName.EndsWith(".asset");
        }).ToDictionary(kvp => kvp.Key.Split('/').Last()["skin".Length..^".asset".Length].ToLowerInvariant(), kvp => kvp.Key);
    }

    extension(IAssetProvider assetProvider)
    {
        public Dictionary<string, string> SkinToPath => _skinToPathLutDict;

        public SkinDefMoistureUpsetRemix? LoadSkin(string skinName)
        {
            if (!assetProvider.SkinToPath.TryGetValue(skinName.ToLowerInvariant(), out var skinPath))
                return null;
            
            return assetProvider.Load<SkinDefMoistureUpsetRemix>(skinPath);
        }
    }

    extension(AssetManager)
    {
        public static SkinDefMoistureUpsetRemix? LoadSkin(string skinName) =>
            AssetManager.AssetProvider?.LoadSkin(skinName);
    }
}