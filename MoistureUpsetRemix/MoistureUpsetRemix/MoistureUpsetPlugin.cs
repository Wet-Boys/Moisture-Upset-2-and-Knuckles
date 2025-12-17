using BepInEx;
using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Common.Logging;
using MoistureUpsetRemix.Skins.Enemies;
using MoistureUpsetRemix.Utils;

namespace MoistureUpsetRemix;

[BepInDependency("com.rune580.riskofoptions")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class MoistureUpsetPlugin : BaseUnityPlugin
{
    public void Awake()
    {
        Log.SetLogger(new Logger(Logger));
        AssetManager.SetAssetProvider(new AssetProvider());
        
        var beetle = new Beetle();
        beetle.Apply();
        var bison = new Bison();
        bison.Apply();
        var lemurian = new Lemurian();
        lemurian.Apply();
    }
}