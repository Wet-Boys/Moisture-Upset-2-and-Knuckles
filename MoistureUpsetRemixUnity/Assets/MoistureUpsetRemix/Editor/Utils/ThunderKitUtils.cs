using System.Linq;
using ThunderKit.Core.Data;
using UnityEditor;

namespace MoistureUpsetRemix.Editor.Utils
{
    public static class ThunderKitUtils
    {
        private static ThunderKitSettings _settings;

        public static ThunderKitSettings Settings
        {
            get
            {
                if (!_settings || _settings == null)
                {
                    var settingsPath = AssetDatabase.FindAssets($"t:{nameof(ThunderKitSettings)}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .FirstOrDefault();
                    
                    _settings = AssetDatabase.LoadAssetAtPath<ThunderKitSettings>(settingsPath);
                }
                
                return _settings;
            }
        }

        public static string GamePath => Settings.GamePath;
    }
}