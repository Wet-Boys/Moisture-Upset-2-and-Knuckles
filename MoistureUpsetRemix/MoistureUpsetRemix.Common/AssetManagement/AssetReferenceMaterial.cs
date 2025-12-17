using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoistureUpsetRemix.Common.AssetManagement;

[Serializable]
public class AssetReferenceMaterial(string guid) : AssetReferenceT<Material>(guid);