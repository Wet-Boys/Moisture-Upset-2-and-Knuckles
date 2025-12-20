using UnityEngine;

namespace MoistureUpsetRemix.Runtime
{
    [CreateAssetMenu(fileName = "ShaderMapping", menuName = "MoistureUpsetRemix/ShaderMapping", order = 3)]
    public class ShaderMapping : ScriptableObject
    {
        public string fromShaderName;
        public Shader toShader;

        public bool Matches(Material material)
        {
            if (!material || !material.shader)
                return false;
            
            Debug.Log(material.shader.name);
            return material.shader.name == fromShaderName;
        }

        public Material SwapShader(Material originalMat)
        {
            var newMat = new Material(toShader);
            newMat.CopyPropertiesFromMaterial(originalMat);
            return newMat;
        }
    }
}