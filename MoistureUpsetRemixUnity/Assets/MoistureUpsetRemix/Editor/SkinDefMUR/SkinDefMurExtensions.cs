using MoistureUpsetRemix.Common.Utils;
using MoistureUpsetRemix.Skins;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using AddressableUtils = MoistureUpsetRemix.Editor.Utils.AddressableUtils;

namespace MoistureUpsetRemix.Editor.SkinDefMUR
{
    internal static class SkinDefMurExtensions
    {
        public static bool IsValid(this ModelSkinControllerProxy msc)
        {
            if (!AddressableUtils.Exists(msc.prefabAddressablePath))
                return false;

            var prefab = Addressables.LoadAssetAsync<GameObject>(msc.prefabAddressablePath).WaitForCompletion();

            try
            {
                var mscTransform = prefab.transform.Find(msc.mscPath);
                if (!mscTransform)
                    return false;

                if (!mscTransform.GetComponent<ModelSkinController>())
                    return false;
            }
            finally
            {
                Resources.UnloadUnusedAssets();
            }
            
            return true;
        }

        public static ModelSkinController GetModelSkinController(this ModelSkinControllerProxy msc)
        {
            if (!AddressableUtils.Exists(msc.prefabAddressablePath))
                return null;

            var prefab = Addressables.LoadAssetAsync<GameObject>(msc.prefabAddressablePath).WaitForCompletion();

            try
            {
                var mscTransform = prefab.transform.Find(msc.mscPath);
                if (!mscTransform)
                    return null;

                return mscTransform.GetComponent<ModelSkinController>();
            }
            finally
            {
                Resources.UnloadUnusedAssets();
            }
        }

        public static ManagedAddressableAsset<GameObject> GetPrefab(this ModelSkinControllerProxy msc)
        {
            if (!AddressableUtils.Exists(msc.prefabAddressablePath))
                return null;
            
            var handle = Addressables.LoadAssetAsync<GameObject>(msc.prefabAddressablePath);
            return new ManagedAddressableAsset<GameObject>(handle);
        }
    }
}