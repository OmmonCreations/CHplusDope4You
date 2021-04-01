using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using RuntimeAssetImporter;

namespace DopeElections.Accounts
{
    public class ZipTable
    {
        private AssetPack Assets { get; }

        private readonly Dictionary<int, Canton> _cantonMap = new Dictionary<int, Canton>();

        private ZipTable(AssetPack assets)
        {
            Assets = assets;
        }

        public bool TryGetCanton(string zipString, out Canton canton)
        {
            var zip = int.TryParse(zipString, out var z) ? z : 0;
            if (zip != 0) return _cantonMap.TryGetValue(zip, out canton);
            canton = null;
            return false;

        }

        private void Load(string data)
        {
            var lines = data.Split('\n');
            const int zipColumn = 0;
            const int countryColumn = 6;
            const int cantonColumn = 5;
            const int cantonDeColumn = 2;
            const int cantonFrColumn = 3;
            const int cantonItColumn = 4;

            var cantonAbbreviationMap = new Dictionary<string, Canton>();
            var cantons = Assets.GetAssets<Canton>();

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var parts = line.Split(',');
                if (parts.Length <= 1) continue;
                var country = parts[countryColumn];
                if (country != "Schweiz") continue;
                var zip = int.TryParse(parts[zipColumn], out var z) ? z : 0;
                if (zip == 0) continue;
                var cantonAbbreviation = parts[cantonColumn];
                Canton canton;
                if (cantonAbbreviationMap.ContainsKey(cantonAbbreviation))
                {
                    canton = cantonAbbreviationMap[cantonAbbreviation];
                }
                else
                {
                    var cantonDe = parts[cantonDeColumn];
                    var cantonFr = parts[cantonFrColumn];
                    var cantonIt = parts[cantonItColumn];
                    canton = cantons.FirstOrDefault(
                        c => c.name == cantonDe || c.name == cantonFr || c.name == cantonIt
                    );
                    if (canton == null)
                    {
                        continue;
                    }

                    cantonAbbreviationMap[cantonAbbreviation] = canton;
                }

                _cantonMap[zip] = canton;
            }
        }

        internal static ZipTable Load()
        {
            var app = DopeElectionsApp.Instance;
            var result = new ZipTable(app.Assets);
            app.InternalStorage.ReadAllText("data/zip-codes.csv", data =>
            {
                result.Load(data);
            });
            return result;
        }
    }
}