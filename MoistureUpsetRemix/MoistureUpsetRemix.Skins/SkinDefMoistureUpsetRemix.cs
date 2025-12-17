using System;
using System.Collections.Generic;
using System.Linq;
using MoistureUpsetRemix.Common.Materials;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoistureUpsetRemix.Skins;

[CreateAssetMenu(fileName = "SkinDef", menuName = "MoistureUpsetRemix/SkinDef", order = 1)]
public class SkinDefMoistureUpsetRemix : ScriptableObject
{
    public ModelSkinControllerProxy msc;
    public string targetSkinName = "";

    public List<SkinnedMeshRendererOverride> smrOverrides = [];

    public void ReplaceEnemySkin()
    {
        var prefabHandle = Addressables.LoadAssetAsync<GameObject>(msc.prefabAddressablePath);

        try
        {
            var prefab = prefabHandle.WaitForCompletion();
            if (!prefab)
                return;

            var mscTransform = prefab.transform.Find(msc.mscPath);
            if (!mscTransform)
                return;

            var modelSkinController = mscTransform.GetComponent<ModelSkinController>();
            foreach (var skin in modelSkinController.skins)
            {
                if (skin.name != targetSkinName)
                    continue;

                var paramsRef = skin.skinDefParamsAddress;

                var skinDefParams = paramsRef is null
                    ? skin.skinDefParams
                    : paramsRef.Asset
                        ? paramsRef.Asset as SkinDefParams
                        : paramsRef.LoadAssetAsync().WaitForCompletion();

                if (!skinDefParams)
                    continue;

                skinDefParams!.meshReplacements = smrOverrides.Where(x => x.IsValid())
                    .Select(x =>
                    {
                        var renderer = prefab.transform.Find(x.smrPath).GetComponent<SkinnedMeshRenderer>();
                        return new SkinDefParams.MeshReplacement
                        {
                            renderer = renderer,
                            mesh = x.enableMeshOverride ? x.meshOverride : null
                        };
                    }).ToArray();

                skinDefParams.rendererInfos = smrOverrides.Where(x => x.IsValid())
                    .Where(x => x.materialOverride)
                    .Select(x =>
                    {
                        var renderer = prefab.transform.Find(x.smrPath).GetComponent<SkinnedMeshRenderer>();
                        return new CharacterModel.RendererInfo
                        {
                            renderer = renderer,
                            defaultMaterial = x.materialOverride!.ConvertToHGStandard()
                        };
                    }).ToArray();
            }
        }
        finally
        {
            prefabHandle.Release();
        }
    }

    [Serializable]
    public struct SkinnedMeshRendererOverride
    {
        public string smrPath;
        public bool enableMeshOverride;
        public Mesh meshOverride;
        public HGStandardOverride? materialOverride;

        public bool IsValid() => !string.IsNullOrEmpty(smrPath);
    }
}