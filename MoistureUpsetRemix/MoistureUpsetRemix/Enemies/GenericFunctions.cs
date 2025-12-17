using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Common.Logging;
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
        // currentSmr.sharedMesh = Assets.Load<Mesh>(mesh);
        var texture = AssetManager.Load<Texture>(png);
        // DebugClass.Log($"shared mats length: {currentSmr.sharedMaterials.Length}");
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
        // foreach (var info in targetModel.baseRendererInfos)
        // {
        //     info.defaultMaterial.mainTexture = texture;
        // }
            
        //Can't seem to find a place to replace the material
        var controller = fab.GetComponentInChildren<ModelSkinController>();
        Log.Debug($"===================== {controller.skins.Length}");
        foreach (var skin in controller.skins)
        {
            // var result = skin.skinDefParamsAddress.LoadAssetAsync().Result;
            Log.Debug($"===================== 1 {skin}   2 {skin.skinDefParams}  3 {skin.optimizedSkinDefParams}  4 {skin.optimizedSkinDefParamsAddress.LoadAssetAsync().Result}");
            // foreach (var info in skin.skinDefParamsAddress.LoadAssetAsync().Result.rendererInfos)
            // {
            //     DebugClass.Log($"===================== info: {info.defaultMaterialAddress.LoadAssetAsync().Result}");
            // }
        }

        var targetModel = fab.GetComponentInChildren<CharacterModel>();
        var defaultParams = Addressables.LoadAssetAsync<SkinDefParams>("RoR2/Base/Beetle/skinBeetleDefault_params.asset").WaitForCompletion();
        var meshReplacement = new SkinDefParams.MeshReplacement();
        var rendererReplacement = new CharacterModel.RendererInfo();
        meshReplacement.renderer = currentSmr;
        meshReplacement.mesh = AssetManager.Load<Mesh>(mesh);
        rendererReplacement.renderer = currentSmr;
        rendererReplacement.defaultMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Beetle/matBeetle.mat").WaitForCompletion();
        rendererReplacement.defaultMaterial.mainTexture = texture;
        defaultParams.meshReplacements = new[] { meshReplacement };
        defaultParams.rendererInfos = new[] { rendererReplacement };
    }
}