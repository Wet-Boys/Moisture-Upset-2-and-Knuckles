using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using MoistureUpsetRemix.Common.Logging;
using MoistureUpsetRemix.Common.Utils;
using MoistureUpsetRemix.Skins;
using MoistureUpsetRemix.Skins.Enemies;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using Object = UnityEngine.Object;

namespace MoistureUpsetRemix.Enemies;

public static class EnemySkinReplacer
{
    private static readonly Dictionary<string, BaseEnemyReplacement> EnemyReplacements = new();
    public static readonly Dictionary<string, SkinDefMoistureUpsetRemix> EnemySkinDef = new();
    
    public static readonly Dictionary<string, ConfigEntry<bool>> EnemyConfigEntries = new();

    private static readonly HashSet<Type> EnemyReplacementTypes = [];

    public static void Init(ConfigFile config)
    {
        // var enemySkins = AppDomain.CurrentDomain.GetAssemblies()
        //     .SelectMany(assembly => assembly.GetTypesSafe())
        //     .Where(t => typeof(BaseEnemyReplacement).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var enemySkin in EnemyReplacementTypes)
        {
            Log.Debug(enemySkin.Name);
            
            var instance = (BaseEnemyReplacement)Activator.CreateInstance(enemySkin);
            
            Log.Debug($"Loading SkinReplacement for {enemySkin.Name}");

            var skinDef = instance.SkinDef;

            skinDef.AddEnemySkinToPrefab();
            EnemyReplacements[skinDef.targetSkinName] = instance;
            EnemySkinDef[skinDef.targetSkinName] = skinDef;
            
            var configEntry = config.Bind("Enemy", instance.ConfigName, true, instance.ConfigDescription);

            configEntry.SettingChanged += (sender, args) =>
            {
                foreach (var modelSkinController in Object.FindObjectsOfType<ModelSkinController>())
                    modelSkinController.ApplySkin(modelSkinController.currentSkinIndex);
            };

            EnemyConfigEntries[instance.ConfigName] = configEntry;
            
            ModSettingsManager.AddOption(new CheckBoxOption(configEntry));
        }
    }

    public static void Register<T>()
        where T : BaseEnemyReplacement
    {
        var type = typeof(T);
        EnemyReplacementTypes.Add(type);
    }

    public static bool IsSkinEnabled(string targetSkin)
    {
        if (!EnemyReplacements.TryGetValue(targetSkin, out var instance))
            return false;
        
        return EnemyConfigEntries[instance.ConfigName].Value;
    }
}