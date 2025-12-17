using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace MoistureUpsetRemix.Common.Utils;

public class ManagedAddressableAsset<T>(AsyncOperationHandle<T> handle) : IDisposable
    where T : Object
{
    private AsyncOperationHandle<T> _handle = handle;
    private bool _disposed;
    
    private T? _asset;
    
    public T Asset => _asset ? _asset! : GetAndCacheAsset();

    private T GetAndCacheAsset()
    {
        if (_disposed)
            throw new ObjectDisposedException("ManagedAddressableAsset already disposed!");
        
        if (!_asset)
            _asset = _handle.WaitForCompletion();
        
        return _asset!;
    }
    
    public void Dispose()
    {
        _handle.Release();
        _asset = null;
        _disposed = true;
    }
    
    public static implicit operator T(ManagedAddressableAsset<T> managedAsset) => managedAsset.Asset;
}