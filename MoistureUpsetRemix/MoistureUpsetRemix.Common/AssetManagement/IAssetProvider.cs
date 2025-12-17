using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoistureUpsetRemix.Common.AssetManagement;

public interface IAssetProvider
{
    public List<AssetBundle> AssetBundles { get; }
    
    public Dictionary<string, int> AssetIndices { get; }
    
    public void PopulateAssets();
    
    public T? Load<T>(string assetName) where T : Object;
}