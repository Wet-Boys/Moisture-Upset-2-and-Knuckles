using System.Collections.Generic;
using System.Linq;
using MoistureUpsetRemix.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace MoistureUpsetRemix.Editor.Utils
{
    public static class MaterialUtils
    {
        private static IEnumerable<ShaderMapping> ShaderMappings => AssetDatabase.FindAssets($"t:{nameof(ShaderMapping)}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ShaderMapping>);
        
        public static Material TrySwapShader(Material originalMat)
        {
            if (!originalMat || !originalMat.shader)
                return originalMat;
            
            foreach (var shaderMapping in ShaderMappings)
            {
                if (!shaderMapping.Matches(originalMat))
                    continue;
                
                return shaderMapping.SwapShader(originalMat);
            }
            
            return originalMat;
        }

        public static void CloneTextures(this Material material)
        {
            if (!material)
                return;
            
            foreach (var texturePropertyName in material.GetTexturePropertyNames())
            {
                var texture = material.GetTexture(texturePropertyName);
                if (texture is Texture2D texture2D)
                {
                    var clone = new Texture2D(texture2D.width, texture2D.height, texture2D.graphicsFormat, texture2D.mipmapCount, TextureCreationFlags.None);
                    
                    Graphics.CopyTexture(texture2D, clone);
                    clone.Apply();
                    material.SetTexture(texturePropertyName, clone);
                }
            }
        }
    }
}