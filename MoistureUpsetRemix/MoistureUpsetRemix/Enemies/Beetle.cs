using System.Collections.Generic;
using RoR2;
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
        foreach (var item in fab.GetComponentsInChildren<Transform>())
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
        foreach (var item in fab.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            item.bones = t.ToArray();
        }
        DebugClass.Log("===================== frog");
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
    }
}