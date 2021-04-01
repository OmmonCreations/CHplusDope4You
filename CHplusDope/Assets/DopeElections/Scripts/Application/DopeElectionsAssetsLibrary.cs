using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Candidates;
using DopeElections.ExtraInfoTalkers;
using DopeElections.Placeholders;
using RuntimeAssetImporter;
using UnityEngine;

namespace DopeElections
{
    [CreateAssetMenu(fileName = "AssetLibrary", menuName = "Dope Elections/Assets Library")]
    public class DopeElectionsAssetsLibrary : AssetsLibrary
    {
        [SerializeField] private ExtraInfoTalkerController[] _talkers = null;
        [SerializeField] private BuiltinCandidate[] _candidates = null;

        public override void LoadInto(AssetPack assetPack)
        {
            assetPack.PutAssets(_talkers);

            LoadBuiltinCandidatesInto(assetPack);
        }

        private void LoadBuiltinCandidatesInto(AssetPack assetPack)
        {
            var candidates = _candidates;
            var portraitMap = new Dictionary<BuiltinCandidate, string>();
            foreach (var c in candidates)
            {
                var sprite = c.Portrait;
                if (!sprite) continue;
                var urlImage = "file://img/candidates/" + c.name + ".png";
                WebSprite.Add(urlImage, sprite);
                portraitMap.Add(c, urlImage);
            }

            assetPack.PutAssets(candidates.Select((c, index) => new Candidate()
            {
                id = 69420000 + index,
                firstName = c.Firstname,
                lastName = c.Lastname,
                urlImage = portraitMap.TryGetValue(c, out var img) ? img : "",
                city = BuiltinCandidate.City
            }).ToArray());
        }
    }
}