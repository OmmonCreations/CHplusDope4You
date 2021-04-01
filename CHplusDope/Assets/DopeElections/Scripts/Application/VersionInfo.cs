using System;
using Newtonsoft.Json.Linq;

namespace DopeElections
{
    public class VersionInfo
    {
        public string Text { get; }
        public string ApiVersion { get; }
        public DateTime ReleaseDate { get; }

        private VersionInfo(string text, string apiVersion, DateTime releaseDate)
        {
            Text = text;
            ApiVersion = apiVersion;
            ReleaseDate = releaseDate;
        }
        
        internal static VersionInfo Load(JObject json)
        {
            var text = json["version_id"] != null ? (string) json["version_id"] : null;
            var apiLevel = json["api_version"] != null ? (string) json["api_version"] : null;
            var releaseDate = json["insertdate"] != null ? new DateTime().AddSeconds(((int) json["insertdate"])) : new DateTime();
            if (text == null || apiLevel == null) return null;
            return new VersionInfo(text, apiLevel, releaseDate);
        }
    }
}