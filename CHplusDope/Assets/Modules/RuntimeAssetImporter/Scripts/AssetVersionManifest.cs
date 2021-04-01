using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace RuntimeAssetImporter
{
    public class AssetVersionManifest
    {
        private Dictionary<string,string> Versions { get; }

        private AssetVersionManifest(Dictionary<string, string> versions)
        {
            Versions = versions;
        }

        public string GetVersion(string assetTypeId)
        {
            return Versions.TryGetValue(assetTypeId, out var result) ? result : null;
        }
        
        internal static AssetVersionManifest Load(JArray versionsArray)
        {
            var versions = versionsArray.OfType<JObject>().Where(e => e["asset_type"] != null && e["version"] != null)
                .ToDictionary(e => (string) e["asset_type"], e => (string) e["version"]);

            return new AssetVersionManifest(versions);
        }
    }
}