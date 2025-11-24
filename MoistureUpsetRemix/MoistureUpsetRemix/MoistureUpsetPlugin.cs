using System;
using BepInEx;
using LeTai.Asset.TranslucentImage;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace MoistureUpsetRemix
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MoistureUpsetPlugin : BaseUnityPlugin
    {
        public static MoistureUpsetPlugin Instance;
        
        public void Awake()
        {
            Instance = this;
            DebugClass.SetLogger(Logger);
        }
    }
}