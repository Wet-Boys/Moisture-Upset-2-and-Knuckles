using System;
using System.Reflection;
using UnityEditor;

namespace MoistureUpsetRemix.Editor.Utils
{
    public static class ParticleSystemUtils
    {
        private static PropertyInfo _previewLayerPropertyInstance;

        private static PropertyInfo PreviewLayerProperty
        {
            get
            {
                if (_previewLayerPropertyInstance is null)
                {
                    var assembly = typeof(AssetDatabase).Assembly;
                    var type = Type.GetType($"UnityEditor.ParticleSystemEditorUtils, {assembly.FullName}");
                    
                    _previewLayerPropertyInstance = type!.GetProperty("previewLayers", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                }

                return _previewLayerPropertyInstance;
            }
        }
        
        public static uint PreviewLayer
        {
            get => (uint)PreviewLayerProperty.GetGetMethod(true).Invoke(null, Array.Empty<object>());
            set => PreviewLayerProperty.GetSetMethod(true).Invoke(null, new object[] { value });
        }

        [Flags]
        public enum PreviewLayerType : uint
        {
            Nothing = 0,
            Default = 1,
            TransparentFx = 2,
            IgnoreRaycast = 4,
            Water = 16,
            UI = 32,
            Everything = 4294967295,
        }
    }
}