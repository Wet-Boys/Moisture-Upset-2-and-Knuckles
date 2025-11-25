using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoistureUpsetRemix.Enemies;

public class GenericFunctions
{
    internal static void ReplaceModel(GameObject fab, string mesh, string png, int position = 0, bool replaceothers = false)
    {
        var meshes = fab.GetComponentsInChildren<SkinnedMeshRenderer>();
        var currentSmr = meshes[position];
        currentSmr.sharedMesh = Assets.Load<Mesh>(mesh);
        var texture = Assets.Load<Texture>(png);
        DebugClass.Log($"shared mats length: {currentSmr.sharedMaterials.Length}");
        //commented for now since it don't work
        // for (int i = 0; i < currentSmr.sharedMaterials.Length; i++)
        // {
        //     currentSmr.sharedMaterials[i].color = Color.white;
        //     currentSmr.sharedMaterials[i].mainTexture = texture;
        //     currentSmr.sharedMaterials[i].SetTexture("_EmTex", null);
        //     currentSmr.sharedMaterials[i].SetTexture("_NormalTex", null);
        //     currentSmr.sharedMaterials[i].SetTexture("_FresnelRamp", null);
        //     currentSmr.sharedMaterials[i].SetTexture("_FlowHeightRamp", null);
        //     currentSmr.sharedMaterials[i].SetTexture("_FlowHeightmap", null);
        // }
        // if (replaceothers)
        // {
        //     for (int i = 0; i < meshes.Length; i++)
        //     {
        //         if (i != position)
        //         {
        //             meshes[i].sharedMesh = Assets.Load<Mesh>(mesh);
        //         }
        //     }
        // }
            
            
        //Can't seem to find a place to replace the material
        var controller = fab.GetComponentInChildren<ModelSkinController>();
        DebugClass.Log($"===================== {controller.skins.Length}");
        foreach (var skin in controller.skins)
        {
            DebugClass.Log($"===================== 1 {skin}   2 {skin.skinDefParamsAddress.LoadAssetAsync().Result}");
        }

        var targetModel = fab.GetComponentInChildren<CharacterModel>();
        DebugClass.Log($"========================== {targetModel.baseRendererInfos.Length}");
        foreach (var info in targetModel.baseRendererInfos)
        {
            info.defaultMaterial.mainTexture = texture;
        }
    }
}