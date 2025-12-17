using System;
using System.IO;
using BepInEx.Bootstrap;
using MoistureUpsetRemix.Common.Logging;

namespace MoistureUpsetRemix;

internal static class FsUtils
{
    private static string? _assetBundlesDir;

    public static string AssetBundlesDir
    {
        get
        {
            _assetBundlesDir ??= GetAssetBundlesDir();

            if (string.IsNullOrEmpty(_assetBundlesDir))
                return "";
            
            return _assetBundlesDir;
        }
    }

    private static string? GetAssetBundlesDir()
    {
        if (!Chainloader.PluginInfos.TryGetValue(MyPluginInfo.PLUGIN_GUID, out var pluginInfo))
            return null;
        
        var dllLoc = pluginInfo.Location;
        var parentDir = Directory.GetParent(dllLoc);

        if (parentDir is null)
            throw new NotSupportedException(BadInstallError());

        string assetBundlesDir = Path.Combine(parentDir.FullName, "AssetBundles");
        if (!Directory.Exists(assetBundlesDir))
            throw new NotSupportedException(BadInstallError());

        return assetBundlesDir;

        string BadInstallError()
        {
            const string msg = "MoistureUpset can't find it's required AssetBundles! This will cause many issues!\nThis either means your mod manager incorrectly installed MoistureUpset" +
                               "or if you've manually installed MoistureUpset, you've done so incorrectly.";
            
            Log.Error(msg);
            return msg;
        }
    }
}