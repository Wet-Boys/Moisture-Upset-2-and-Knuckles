using System.Collections.Generic;
using MoistureUpsetRemix.Common.Materials;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace MoistureUpsetRemix.Editor.SkinDefMUR
{
    [CustomEditor(typeof(HGStandardOverride))]
    public class HGStandardMockEditor : UnityEditor.Editor
    {
        private HGStandardOverride Target => (HGStandardOverride)target;

        private MaterialEditor _materialEditor;
        private Material _combinedMaterial;
        private Dictionary<string, object> _originalValues = new();

        private void OnEnable()
        {
            _combinedMaterial = Target.GetCombinedMaterial(
                AssetDatabase.LoadAssetAtPath<Shader>("Assets/MoistureUpsetRemix/Shaders/HGStandardMock.shader"));
            if (!_combinedMaterial)
                return;
            
            CacheProperties();

            _materialEditor = (MaterialEditor)CreateEditor(_combinedMaterial);
        }

        private void OnDisable()
        {
            if (_materialEditor)
            {
                DestroyImmediate(_materialEditor);
                _materialEditor = null;
            }

            if (_combinedMaterial)
            {
                DestroyImmediate(_combinedMaterial, true);
                _combinedMaterial = null;
            }
            
            _originalValues.Clear();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            if (_materialEditor)
            {
                _materialEditor.DrawHeader();

                _materialEditor.OnInspectorGUI();
            }

            if (EditorGUI.EndChangeCheck())
            {
                ApplyChanges();
            }
        }

        private void CacheProperties()
        {
            _originalValues.Clear();
            
            var originalMat = Addressables.LoadAssetAsync<Material>(Target.originalMaterialAddressablePath).WaitForCompletion();
            
            if (!originalMat)
                return;

            var shader = originalMat.shader;
            
            var values = new Dictionary<string, object>();

            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                var propName = shader.GetPropertyName(i);
                var propType = shader.GetPropertyType(i);
                
                // Set up default value dict
                switch (propType)
                {
                    case ShaderPropertyType.Color:
                        values[propName] = originalMat.GetColor(propName);
                        break;
                    case ShaderPropertyType.Vector:
                        values[propName] = originalMat.GetVector(propName);
                        break;
                    case ShaderPropertyType.Float or ShaderPropertyType.Range:
                        values[propName] = originalMat.GetFloat(propName);
                        break;
                    case ShaderPropertyType.Texture:
                        values[propName] = originalMat.GetTexture(propName);
                        break;
                }
            }

            _originalValues = values;
        }

        private void ApplyChanges()
        {
            if (!_combinedMaterial || !Target.overrideMaterial)
                return;

            var shader = _combinedMaterial.shader;
            var dirty = false;
            
            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                var propName = shader.GetPropertyName(i);
                var propType = shader.GetPropertyType(i);
                
                // Set up default value dict
                switch (propType)
                {
                    case ShaderPropertyType.Color:
                        var newColor = _combinedMaterial.GetColor(propName);
                        var originalColor = (Color)_originalValues[propName];
                        if (newColor != originalColor)
                        {
                            Target.overrideMaterial.SetColor(propName, newColor);
                            dirty = true;
                        }
                        break;
                    case ShaderPropertyType.Vector:
                        var newVector = _combinedMaterial.GetVector(propName);
                        var originalVector = (Vector4)_originalValues[propName];
                        if (newVector != originalVector)
                        {
                            Target.overrideMaterial.SetVector(propName, newVector);
                            dirty = true;
                        }
                        break;
                    case ShaderPropertyType.Float or ShaderPropertyType.Range:
                        var newFloat = _combinedMaterial.GetFloat(propName);
                        var originalFloat = (float)_originalValues[propName];
                        if (newFloat != originalFloat)
                        {
                            Target.overrideMaterial.SetFloat(propName, newFloat);
                            dirty = true;
                        }
                        break;
                    case ShaderPropertyType.Texture:
                        var newTexture = _combinedMaterial.GetTexture(propName);
                        var originalTexture = (Texture)_originalValues[propName];
                        if (newTexture != originalTexture)
                        {
                            Target.overrideMaterial.SetTexture(propName, newTexture);
                            dirty = true;
                        }
                        break;
                }
            }
            
            if (dirty)
            {
                EditorUtility.SetDirty(Target.overrideMaterial);
                EditorUtility.SetDirty(Target);
                AssetDatabase.SaveAssetIfDirty(Target.overrideMaterial);
                AssetDatabase.SaveAssetIfDirty(Target);
            }
        }
    }
}