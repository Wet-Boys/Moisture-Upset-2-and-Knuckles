using System;
using System.Collections.Generic;
using System.Linq;
using MoistureUpsetRemix.Common.Logging;
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

    public int SkinIndex { get; private set; } = -1;

    private SkinDef? _skinDef;

    public void AddEnemySkinToPrefab()
    {
        var skinDef = AsSkinDef();

        var prefabHandle = Addressables.LoadAssetAsync<GameObject>(msc.prefabAddressablePath);

        try
        {
            var prefab = prefabHandle.WaitForCompletion();
            if (!prefab)
            {
                Log.Error($"[{name}] Failed to find prefab at path: " + msc.prefabAddressablePath);
                return;
            }

            var mscTransform = prefab.transform.Find(msc.mscPath);
            if (!mscTransform)
            {
                Log.Error($"[{name}] Failed to find ModelSkinController at path: " + msc.mscPath);
                return;
            }

            var modelSkinController = mscTransform.GetComponent<ModelSkinController>();
            SkinIndex = modelSkinController.skins.Length;
            Log.Debug($"[{name}] SkinIndex: {SkinIndex}");
            Array.Resize(ref modelSkinController.skins, modelSkinController.skins.Length + 1);
            modelSkinController.skins[^1] = skinDef;
        }
        finally
        {
            prefabHandle.Release();
        }
    }
    
    public SkinDef? AsSkinDef()
    {
        if (_skinDef)
            return _skinDef;

        var prefabHandle = Addressables.LoadAssetAsync<GameObject>(msc.prefabAddressablePath);

        try
        {
            var prefab = prefabHandle.WaitForCompletion();
            if (!prefab)
                return null;

            var mscTransform = prefab.transform.Find(msc.mscPath);
            if (!mscTransform)
                return null;

            var modelSkinController = mscTransform.GetComponent<ModelSkinController>();
            foreach (var skin in modelSkinController.skins)
            {
                if (skin.name != targetSkinName)
                    continue;

                var newSkin = Instantiate(skin);
                var paramsRef = newSkin.skinDefParamsAddress;

                var skinDefParams = paramsRef is null
                    ? skin.skinDefParams
                    : paramsRef.Asset
                        ? paramsRef.Asset as SkinDefParams
                        : paramsRef.LoadAssetAsync().WaitForCompletion();

                if (!skinDefParams)
                    continue;

                if (skinDefParams!.meshReplacements.Length == 0)
                {
                    skinDefParams.meshReplacements = modelSkinController.GetComponentsInChildren<SkinnedMeshRenderer>()
                        .Select(smr => new SkinDefParams.MeshReplacement
                        {
                            renderer = smr,
                            mesh = smr.sharedMesh
                        }).ToArray();
                }

                var newParams = Instantiate(skinDefParams);
                newSkin.skinDefParamsAddress = new AssetReferenceT<SkinDefParams>(null);
                newSkin.skinDefParams = newParams;

                newSkin.optimizedSkinDefParams = newParams;
                newSkin.optimizedSkinDefParamsAddress = new AssetReferenceT<SkinDefParams>(null);

                newParams!.meshReplacements = smrOverrides.Where(x => x.IsValid())
                    .Select(x =>
                    {
                        var renderer = prefab.transform.Find(x.smrPath).GetComponent<SkinnedMeshRenderer>();
                        return new SkinDefParams.MeshReplacement
                        {
                            renderer = renderer,
                            mesh = x.enableMeshOverride ? x.meshOverride : null
                        };
                    }).ToArray();

                newParams.rendererInfos = smrOverrides.Where(x => x.IsValid())
                    .Where(x => x.materialOverride)
                    .Select(x =>
                    {
                        var renderer = prefab.transform.Find(x.smrPath).GetComponent<SkinnedMeshRenderer>();
                        return new CharacterModel.RendererInfo
                        {
                            renderer = renderer,
                            defaultMaterial = x.materialOverride!.GetOverridenMaterial()
                        };
                    }).ToArray();

                _skinDef = newSkin;
                return _skinDef;
            }
        }
        finally
        {
            prefabHandle.Release();
        }

        return null;
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