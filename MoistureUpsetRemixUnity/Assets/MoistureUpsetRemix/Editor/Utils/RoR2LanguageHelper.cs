using System.Collections.Generic;
using System.IO;
using System.Reflection;
using RoR2;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Path = System.IO.Path;

namespace MoistureUpsetRemix.Editor.Utils
{
    public static class RoR2LanguageHelper
    {
        private static bool _initialized;
        
        private static void Init()
        {
            if (_initialized)
                return;
            
            var gamePath = ThunderKitUtils.GamePath;

            CreateEnglishLanguage(Path.Combine(gamePath, "Risk of Rain 2_Data", "StreamingAssets", "Language"));
            
            _initialized = true;
            
        }

        private static void CreateEnglishLanguage(string languageDir)
        {
            var method = typeof(Language).GetMethod("GetOrCreateLanguage", BindingFlags.NonPublic | BindingFlags.Static);
            var lang = method!.Invoke(null, new object[] { "en" });
            
            var englishProp = typeof(Language).GetProperty(nameof(Language.english), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            englishProp!.SetValue(null, lang);
            
            var currentLanguageProp = typeof(Language).GetProperty(nameof(Language.currentLanguage), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            currentLanguageProp!.SetValue(null, lang);

            var englishDir = Path.Combine(languageDir, "en");
            var foldersField = typeof(Language).GetField("folders", BindingFlags.NonPublic | BindingFlags.Instance);
            foldersField!.SetValue(lang, new[] { englishDir });
            
            var tokenStringPairs = new List<KeyValuePair<string, string>>();
            foreach (var file in Directory.EnumerateFiles(englishDir, "*.json", SearchOption.AllDirectories))
            {
                if (Path.GetFileNameWithoutExtension(file) == "language")
                    continue;
                
                var contents = File.ReadAllText(file);
                LoadTokensFromData(contents, tokenStringPairs);
            }
            
            var englishLang = (Language)lang;
            englishLang.SetStringsByTokens(tokenStringPairs);
        }

        private static void LoadTokensFromData(string contents, List<KeyValuePair<string, string>> output)
        {
            var method = typeof(Language).GetMethod("LoadTokensFromData", BindingFlags.NonPublic | BindingFlags.Static);
            method!.Invoke(null, new object[] { contents, output });
        }

        public static string GetString(string token)
        {
            try
            {
                if (!_initialized)
                    Init();

                return Language.GetString(token);
            }
            catch
            {
                return token;
            }
        }
    }
}