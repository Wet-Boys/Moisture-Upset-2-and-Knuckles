using System.Collections.Generic;
using System.Linq;
using MoistureUpsetRemix.Common.Logging;
using MoistureUpsetRemix.Common.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoistureUpsetRemix.Enemies;

public class Beetle
{
    internal static void ReplaceAll()
    {
        //return if settings are off
        ReplaceModel();
        ReplaceAudio();
        ReplaceUI();
        ReplaceOther();
    }
    internal static void ReplaceModel()
    {
        var fab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleBody.prefab").WaitForCompletion();
        List<Transform> t = new List<Transform>();
        //this is the fucking stupid but it works (minus claws)
        var transforms = fab.GetComponentsInChildren<Transform>();
        Log.Debug(transforms.Select(t => t.name).ToDebugString());
        
        foreach (var item in transforms)
        {
            if (!item.name.Contains("Hurtbox") && !item.name.Contains("BeetleBody") && !item.name.Contains("Mesh") && !item.name.Contains("mdl"))
            {
                t.Add(item);
            }
        }
        Transform temp = t[14];
        t[14] = t[11];
        t[11] = temp;
        
        temp = t[15];
        t[15] = t[12];
        t[12] = temp;
        
        temp = t[16];
        t[16] = t[13];
        t[13] = temp;

        var newBones = t.ToArray();
        
        Log.Info(newBones.Select(t => t.name).ToDebugString());
        
        foreach (var item in fab.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            item.bones = newBones;
        }
        Log.Debug("===================== frog");
        GenericFunctions.ReplaceModel(fab, "@MoistureUpset_frog:assets/frogchair.mesh", "@MoistureUpset_frog:assets/frogchair.png");
    }

    internal static void ReplaceAudio()
    {
        //change bark events and such
    }

    internal static void ReplaceUI()
    {
        //change dictionary terms, UI icons, etc.
    }

    internal static void ReplaceOther()
    {
        //Other components if needed. For example, roblox golems randomize their skin on spawn and flying lunar golems spin their bone every tick. Add those components and do other functions here.
        
        // var fab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBody.prefab").WaitForCompletion();
        // GenericFunctions.ReplaceModel(fab, "@MoistureUpset_frog:assets/mike.mesh", "@MoistureUpset_frog:assets/mike.png");
    }
}