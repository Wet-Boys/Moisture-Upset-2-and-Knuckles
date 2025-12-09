using System;
using System.IO;
using MoistureUpsetRemix;

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
        string BadInstallError()
        {
            var modName = MyPluginInfo.PLUGIN_NAME;
            var dllName = $"{nameof(MoistureUpsetPlugin)}.dll";
            var msg = $"{modName} can't find it's required AssetBundles!\n Make sure that the file `{dllName}` is in its own folder like `BepInEx/plugins/{modName}/{dllName}` and that the folder `AssetBundles` is in the same folder as `{dllName}`!";
            
            MoistureUpsetPlugin.Logger?.LogError(msg);
            return msg;
        }
        
        var pluginInfo = MoistureUpsetPlugin.PluginInfo;
        if (pluginInfo is null)
            return null;
        
        var dllLoc = pluginInfo.Location;
        var parentDir = Directory.GetParent(dllLoc);

        if (parentDir is null)
            throw new NotSupportedException(BadInstallError());

        string assetBundlesDir = Path.Combine(parentDir.FullName, "AssetBundles");
        if (!Directory.Exists(assetBundlesDir))
            throw new NotSupportedException(BadInstallError());

        return assetBundlesDir;
    }
}