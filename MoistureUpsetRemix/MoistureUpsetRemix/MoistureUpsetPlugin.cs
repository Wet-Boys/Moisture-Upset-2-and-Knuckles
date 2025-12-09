using BepInEx;
using BepInEx.Logging;
using MoistureUpsetRemix.Enemies;

namespace MoistureUpsetRemix
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class MoistureUpsetPlugin : BaseUnityPlugin
    {
        public static MoistureUpsetPlugin Instance;
        public static BepInEx.PluginInfo PluginInfo { get; private set; }
        public new static ManualLogSource? Logger { get; private set; }
        
        public void Awake()
        {
            PluginInfo = Info;
            Logger = base.Logger;
            Instance = this;
            DebugClass.SetLogger(Logger);
            Assets.PopulateAssets();
            
            // On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += (orig, self, menu) =>
            // {
            //     orig(self, menu);
            // };
            Beetle.ReplaceModel();
        }
    }
}