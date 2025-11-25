using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoistureUpsetRemix.Enemies;

public class Beetle
{
    internal static void Replace()
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
        GenericFunctions.ReplaceModel("RoR2/Base/Beetle/BeetleBody.prefab", "@MoistureUpset_frog:assets/frogchair.mesh", "@MoistureUpset_frog:assets/frogchair.png");
        GenericFunctions.ReplaceModel("RoR2/Base/Lemurian/LemurianBody.prefab", "@MoistureUpset_mike:assets/mike.mesh", "@MoistureUpset_mike:assets/mike.png");
    }
}