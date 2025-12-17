using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace MoistureUpsetRemix.Editor.Utils
{
    public static class AddressableUtils
    {
        private static IReadOnlyCollection<IResourceLocation> _locations;

        public static IReadOnlyCollection<IResourceLocation> Locations
        {
            get
            {
                if (_locations is not null && _locations.Count != 0)
                    return _locations;
                
                var set = new HashSet<IResourceLocation>();
                var validLocations = Addressables.ResourceLocators.OfType<ResourceLocationMap>()
                    .SelectMany(rlm =>
                        rlm.Locations.SelectMany(loc => loc.Value)
                            .Where(val => val.ResourceType != typeof(IAssetBundleResource)));

                foreach (var location in validLocations)
                    set.Add(location);
                
                _locations = new ReadOnlyCollection<IResourceLocation>(set.ToList());
                
                return _locations;
            }
        }

        public static bool Exists(string addressablePath) =>
            Locations.Any(location => location.PrimaryKey == addressablePath);
        
        public static bool IsLoadableAsset(this IResourceLocation location) =>
            location.ResourceType != typeof(SceneInstance)
            && location.ResourceType != typeof(IAssetBundleResource)
            && location.ProviderId != "UnityEngine.ResourceManagement.ResourceProviders.LegacyResourcesProvider"
            && typeof(Object).IsAssignableFrom(location.ResourceType);

        public static bool IsMaterialAsset(this IResourceLocation location) =>
            location.ResourceType == typeof(Material)
            && location.ProviderId != "UnityEngine.ResourceManagement.ResourceProviders.LegacyResourcesProvider";
        
        public static MethodInfo GetLoadMethod(this IResourceLocation location)
        {
            var addressablesType = typeof(Addressables);
            var loadAsyncMethodTemplate = addressablesType.GetMethod(nameof(Addressables.LoadAssetAsync), new [] { typeof(IResourceLocation)});
            var loadAssetAsync = loadAsyncMethodTemplate?.MakeGenericMethod(location.ResourceType);
            return loadAssetAsync;
        }

        public static T GetOrLoadAsset<T>(this AssetReferenceT<T> assetReference)
            where T : Object
        {
            if (assetReference.Asset)
                return assetReference.Asset as T;

            return assetReference.LoadAssetAsync().WaitForCompletion();
        }
    }
}