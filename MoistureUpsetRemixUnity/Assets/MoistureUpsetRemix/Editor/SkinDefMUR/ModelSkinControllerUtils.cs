using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoistureUpsetRemix.Editor.Utils;
using MoistureUpsetRemix.Skins;
using Newtonsoft.Json;
using RoR2;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Path = System.IO.Path;

namespace MoistureUpsetRemix.Editor.SkinDefMUR
{
    internal static class ModelSkinControllerUtils
    {
        private static string CachePath => Path.GetFullPath(Path.Combine(Path.GetFullPath(Application.dataPath), "../", "model-skin-controllers.json"));
        
        private static ModelSkinControllerInfo[] _infos = Array.Empty<ModelSkinControllerInfo>();

        public static ModelSkinControllerInfo[] GetInfoArray()
        {
            if (_infos.Length == 0)
            {
                if (!File.Exists(CachePath))
                {
                    BuildInfoCache();
                }
                else
                {
                    var contents = File.ReadAllText(CachePath);
                    var cache = JsonConvert.DeserializeObject<ModelSkinControllerInfoCache>(contents);
                    _infos = cache.Infos;
                }
            }
            
            return _infos;
        }

        [MenuItem("Tools/Moisture Upset Remix/Rebuild ModelSkinController Cache")]
        public static void RebuildCache()
        {
            if (File.Exists(CachePath))
                File.Delete(CachePath);
            
            BuildInfoCache();
        }

        private static void BuildInfoCache()
        {
            var infos = new HashSet<ModelSkinControllerInfo>();
            
            foreach (var loc in AddressableUtils.Locations)
            {
                if (!loc.IsLoadableAsset())
                    continue;

                var loadAssetAsync = loc.GetLoadMethod();
                var loadParams = new object[] { loc };
                var firstOp = loadAssetAsync.Invoke(null, loadParams);
                
                var asyncOpType = firstOp.GetType();
                var asyncOpWaitForCompletionMethod = asyncOpType.GetMethod("WaitForCompletion");

                var firstObj = asyncOpWaitForCompletionMethod?.Invoke(firstOp, null);
                if (firstObj is not Object unityObj)
                    continue;
                
                if (unityObj is not GameObject gameObject)
                    continue;

                // We only want to check prefabs that have a CharacterBody
                var characterBody = gameObject.GetComponent<CharacterBody>();
                if (!characterBody)
                    continue;

                var modelLocator = gameObject.GetComponent<ModelLocator>();
                if (!modelLocator)
                    continue;

                // Make sure the prefab has a ModelSkinController as well
                if (!modelLocator.modelTransform)
                    continue;
                
                var msc = modelLocator.modelTransform.GetComponent<ModelSkinController>();
                if (!msc)
                    continue;

                var mscPath = modelLocator.modelTransform.GetPathToParent(gameObject.transform);
                var info = new ModelSkinControllerInfo
                {
                    DisplayName = RoR2LanguageHelper.GetString(characterBody.baseNameToken),
                    Skins = msc.skins.Select(skin => skin.name).ToArray(),
                    Proxy = new ModelSkinControllerProxy
                    {
                        prefabAddressablePath = loc.PrimaryKey,
                        mscPath = mscPath,
                    }
                };

                infos.Add(info);
            }

            Resources.UnloadUnusedAssets();

            _infos = infos.ToArray();
            
            var cache = new ModelSkinControllerInfoCache(_infos);
            var contents = JsonConvert.SerializeObject(cache);
            File.WriteAllText(CachePath, contents);
        }

        public static ModelSkinControllerInfo GetInfo(this ModelSkinControllerProxy proxy) => GetInfoArray().FirstOrDefault(info => info.Proxy == proxy);

        public static Mesh GetOriginalMeshForSkin(this ModelSkinController msc, string skinName, string smrPath)
        {
            foreach (var skin in msc.skins)
            {
                if (skin.name != skinName)
                    continue;
                
                var skinDefParams = skin.skinDefParamsAddress.GetOrLoadAsset();
                
                foreach (var meshReplacement in skinDefParams.meshReplacements)
                {
                    var current = meshReplacement.renderer.transform;
                    while (current.parent)
                        current = current.parent;
                    
                    var rendererPath = meshReplacement.renderer.transform.GetPathToParent(current);
                    if (rendererPath == smrPath)
                    {
                        if (meshReplacement.mesh)
                            return meshReplacement.mesh;
                        
                        return meshReplacement.meshAddress.GetOrLoadAsset();
                    }
                }
            }
            
            return null;
        }

        public static Material GetOriginalMaterialForSkin(this ModelSkinController msc, string skinName, string smrPath)
        {
            foreach (var skin in msc.skins)
            {
                if (skin.name != skinName)
                    continue;
                
                var skinDefParams = skin.skinDefParamsAddress.GetOrLoadAsset();

                foreach (var info in skin.rendererInfos)
                {
                    if (!info.renderer)
                        continue;

                    var rendererPath = info.renderer.transform.GetPathToRoot();
                    if (rendererPath != smrPath)
                        continue;

                    if (info.defaultMaterial)
                        return info.defaultMaterial;

                    return info.defaultMaterialAddress.GetOrLoadAsset();
                }

                foreach (var info in skinDefParams.rendererInfos)
                {
                    if (!info.renderer)
                        continue;

                    var rendererPath = info.renderer.transform.GetPathToRoot();
                    if (rendererPath != smrPath)
                        continue;

                    if (info.defaultMaterial)
                        return info.defaultMaterial;

                    return info.defaultMaterialAddress.GetOrLoadAsset();
                }
            }

            return null;
        }

        public struct ModelSkinControllerInfo : IEquatable<ModelSkinControllerInfo>
        {
            public string DisplayName;
            public string[] Skins;
            public ModelSkinControllerProxy Proxy;

            public bool Equals(ModelSkinControllerInfo other)
            {
                return DisplayName == other.DisplayName && Skins == other.Skins && Proxy.Equals(other.Proxy);
            }

            public override bool Equals(object obj)
            {
                return obj is ModelSkinControllerInfo other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(DisplayName, Proxy);
            }

            public static bool operator ==(ModelSkinControllerInfo left, ModelSkinControllerInfo right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ModelSkinControllerInfo left, ModelSkinControllerInfo right)
            {
                return !left.Equals(right);
            }
        }
        
        [Serializable]
        public struct ModelSkinControllerInfoCache
        {
            public ModelSkinControllerInfo[] Infos { get; set; }

            public ModelSkinControllerInfoCache(ModelSkinControllerInfo[] infos)
            {
                Infos = infos;
            }
        }
    }
}