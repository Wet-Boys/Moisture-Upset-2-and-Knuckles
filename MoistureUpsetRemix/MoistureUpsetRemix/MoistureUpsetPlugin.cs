using System.Reflection;
using BepInEx;
using HarmonyLib;
using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Common.Logging;
using MoistureUpsetRemix.Enemies;
using MoistureUpsetRemix.Skins.Enemies;
using MoistureUpsetRemix.Utils;
using Beetle = MoistureUpsetRemix.Skins.Enemies.Beetle;

namespace MoistureUpsetRemix;

[BepInDependency("com.rune580.riskofoptions")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class MoistureUpsetPlugin : BaseUnityPlugin
{
    private Harmony? _harmony;
    
    public void Awake()
    {
        Log.SetLogger(new Logger(Logger));
        AssetManager.SetAssetProvider(new AssetProvider());

        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
        
        EnemySkinReplacer.Register<Beetle>();
        EnemySkinReplacer.Register<Bison>();
        EnemySkinReplacer.Register<Lemurian>();
        
        EnemySkinReplacer.Init(Config);
    }
}