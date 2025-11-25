using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using LeTai.Asset.TranslucentImage;
using MoistureUpsetRemix.Enemies;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace MoistureUpsetRemix
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(MoistureUpsetRemix.PluginInfo.PLUGIN_GUID, MoistureUpsetRemix.PluginInfo.PLUGIN_NAME, MoistureUpsetRemix.PluginInfo.PLUGIN_VERSION)]
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
            
            
            Beetle.Replace();
        }
    }
}