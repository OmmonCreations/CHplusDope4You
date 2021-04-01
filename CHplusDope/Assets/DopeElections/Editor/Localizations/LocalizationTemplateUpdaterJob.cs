using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DopeElections.Localizations;
using Localizator;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace DopeElections.Editor.Localizations
{
    public class LocalizationTemplateUpdaterJob : MonoBehaviour
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void OnRecompile()
        {
            UpdateLocalizationTemplates();
        }

        [MenuItem("Tools/Localizations/Update Keys")]
        private static void UpdateLocalizationTemplates()
        {
            var keyClass = typeof(LKey);
            var defaultLocalization = new Dictionary<string, string>();
            ExtractEntries(keyClass, defaultLocalization);

            var localizationFilesPath = Path.Combine(Application.streamingAssetsPath, "lang");
            var files = Directory.GetFiles(localizationFilesPath);
            var pattern = new Regex("^([a-zA-Z_-]+)\\.json$");
            var anyChangesDetected = false;
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var filename = fileInfo.Name;
                var match = pattern.Match(filename);
                if (!match.Success) continue;
                var language = match.Groups[1].Value;
                anyChangesDetected |= UpdateLocalizationTemplate(file, language, defaultLocalization);
            }

            if (anyChangesDetected) Debug.Log("Localization files updated.");
        }

        private static bool UpdateLocalizationTemplate(string file, string language,
            Dictionary<string, string> defaultLocalization)
        {
            var jsonString = File.ReadAllText(file);
            JObject json;
            try
            {
                json = JObject.Parse(jsonString);
            }
            catch
            {
                Debug.LogWarning("Localization file " + file + " contains invalid Json.");
                return false;
            }

            var anyChangesDetected = false;
            foreach (var entry in defaultLocalization)
            {
                if (json[entry.Key] != null && !string.IsNullOrWhiteSpace((string) json[entry.Key])) continue;
                anyChangesDetected = true;
                json[entry.Key] = entry.Value;
            }

            foreach (var entry in json.Properties().ToList())
            {
                if (defaultLocalization.ContainsKey(entry.Name)) continue;
                json.Remove(entry.Name);
                anyChangesDetected = true;
            }

            if (anyChangesDetected) File.WriteAllText(file, json.ToString());
            return anyChangesDetected;
        }

        private static void ExtractEntries(Type type, Dictionary<string, string> target)
        {
            var localizationKeyType = typeof(LocalizationKey);
            foreach (var field in type.GetFields())
            {
                if (field.FieldType != localizationKeyType) continue;
                var value = (LocalizationKey) field.GetValue(null);
                target[value.path] = value.fallback;
            }

            foreach (var t in type.GetNestedTypes())
            {
                ExtractEntries(t, target);
            }
        }
    }
}