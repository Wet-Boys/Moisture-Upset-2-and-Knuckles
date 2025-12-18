using System.Collections.Generic;
using System.Linq;
using MoistureUpsetRemix.Common.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoistureUpsetRemix.Common.Materials;

public abstract class BaseMaterialOverride : ScriptableObject
{
    public string originalMaterialAddressablePath = "";
    
    protected ManagedAddressableAsset<Material> OriginalMaterial => Addressables.LoadAssetAsyncManaged<Material>(originalMaterialAddressablePath);
    
    public abstract string MockShaderPath { get; }
    
    [SerializeField]
    [SerializeReference]
    protected List<MaterialPropertyOverride> properties = [];

    public void AddOverride(MaterialPropertyOverride propertyOverride)
    {
        var existing = properties.FirstOrDefault(x => x.propertyName == propertyOverride.propertyName);
        if (existing is not null)
        {
            existing.SetValue(propertyOverride.GetValue());
        }
        else
        {
            properties.Add(propertyOverride);
        }
    }

    public void RemoveOverride(string propertyName)
    {
        var existing = properties.FirstOrDefault(x => x.propertyName == propertyName);
        if (existing is null)
            return;
        
        properties.Remove(existing);
    }

    public Material GetOverridenMaterial()
    {
        using var originalMat = OriginalMaterial;
        var newMat = CombineMaterialUsingShader(originalMat.Asset.shader);
        return newMat;
    }

    internal Material CombineMaterialUsingShader(Shader targetShader)
    {
        using var originalMat = OriginalMaterial;
        
        var newMat = new Material(targetShader);
        newMat.CopyPropertiesFromMaterial(originalMat);

        foreach (var property in properties)
            property.SetOnMaterial(newMat);
        
        return newMat;
    }
}