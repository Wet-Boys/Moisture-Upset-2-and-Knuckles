using System.Collections.Generic;
using MoistureUpsetRemix.Common.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace MoistureUpsetRemix.Common.Materials;

[CreateAssetMenu(fileName = "HGStandardOverride", menuName = "MoistureUpsetRemix/HGStandardOverride", order = 2)]
public class HGStandardOverride : ScriptableObject
{
    // private static Shader Shader => AssetManager.Load<Shader>("Assets/MoistureUpsetRemix/Shaders/HGStandardMock.shader")!;

    public string originalMaterialAddressablePath = "";

    public Material? overrideMaterial;
    
    private ManagedAddressableAsset<Material> OriginalMaterial => Addressables.LoadAssetAsyncManaged<Material>(originalMaterialAddressablePath);

    public Material ConvertToHGStandard()
    {
        using var originalMat = OriginalMaterial;
        var newMat = GetCombinedMaterial(originalMat.Asset.shader);
        return newMat;
    }

    internal Material GetCombinedMaterial(Shader targetShader)
    {
        using var originalMat = OriginalMaterial;
        var originalShader = originalMat.Asset.shader;

        var properties = new Dictionary<string, ShaderPropertyType>();
        var defaultValues = new Dictionary<string, object>();

        for (int i = 0; i < originalShader.GetPropertyCount(); i++)
        {
            var propName = originalShader.GetPropertyName(i);
            var propType = originalShader.GetPropertyType(i);

            properties[propName] = propType;
            
            // Set up default value dict
            switch (propType)
            {
                case ShaderPropertyType.Color or ShaderPropertyType.Vector:
                    defaultValues[propName] = originalShader.GetPropertyDefaultVectorValue(i);
                    break;
                case ShaderPropertyType.Float or ShaderPropertyType.Range:
                    defaultValues[propName] = originalShader.GetPropertyDefaultFloatValue(i);
                    break;
                case ShaderPropertyType.Texture:
                    defaultValues[propName] = originalShader.GetPropertyTextureDefaultName(i);
                    break;
            }
        }

        var newMat = new Material(targetShader);
        newMat.CopyPropertiesFromMaterial(originalMat.Asset);

        foreach (var (propName, propType) in properties)
        {
            switch (propType)
            {
                case ShaderPropertyType.Color:
                    var newColor = overrideMaterial.GetColor(propName);
                    if (!defaultValues.TryGetValue(propName, out var boxedColor) || (boxedColor is Color color && newColor != color))
                        newMat.SetColor(propName, newColor);
                    break;
                case ShaderPropertyType.Vector:
                    var newVector = overrideMaterial.GetVector(propName);
                    if (!defaultValues.TryGetValue(propName, out var boxedVector) || (boxedVector is Vector4 vector && newVector != vector))
                        newMat.SetVector(propName, newVector);
                    break;
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    var newFloat = overrideMaterial.GetFloat(propName);
                    if (!defaultValues.TryGetValue(propName, out var boxedFloat) || (boxedFloat is float f && newFloat != f))
                        newMat.SetFloat(propName, newFloat);
                    break;
                case ShaderPropertyType.Texture:
                    var newTexture = overrideMaterial.GetTexture(propName);
                    if (newTexture is not null)
                        newMat.SetTexture(propName, newTexture);
                    break;
                case ShaderPropertyType.Int:
                    var newInt = overrideMaterial.GetInt(propName);
                    var origInt = newMat.GetInt(propName);
                    if (newInt != 0 && origInt != newInt)
                        newMat.SetInt(propName, newInt);
                    break;
            }
        }
    
        return newMat;
    }
}