using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MoistureUpsetRemix.Common.Utils;

public static class AddressableUtils
{
    extension<T>(AssetReferenceT<T> assetRef)
        where T : Object
    {
        public T? GetOrLoadAsset(out AsyncOperationHandle<T> handle)
        {
            handle = assetRef.LoadAssetAsync();

            if (assetRef.Asset)
                return assetRef.Asset as T;


            return handle.WaitForCompletion();
        }
    }

    extension(Addressables)
    {
        public static ManagedAddressableAsset<T> LoadAssetAsyncManaged<T>(object key)
            where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            return new ManagedAddressableAsset<T>(handle);
        }
    }
}