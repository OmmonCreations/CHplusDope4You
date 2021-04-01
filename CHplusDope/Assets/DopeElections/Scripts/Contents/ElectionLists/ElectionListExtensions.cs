using System.Linq;
using DopeElections.Answer;
using DopeElections.CandidateParties;
using DopeElections.Candidates;
using DopeElections.Localizations;
using DopeElections.Users;
using Localizator;
using UnityEngine;

namespace DopeElections.ElectionLists
{
    public static class ElectionListExtensions
    {
        public static float GetMatchPercentage(this ElectionList list, ActiveUser user)
        {
            var userSpider = user.SmartSpider;
            if (!userSpider.HasData) return float.NaN;

            var listSpider = list.GetSmartSpider();
            var values = userSpider.Values;
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = Mathf.Abs(values[i] - listSpider[i]);
            }

            return values.Average();
        }

        public static Candidate[] GetCandidates(this ElectionList list)
        {
            if (list.candidates == null)
            {
                Debug.LogError("List has no candidates!\n" + list);
                return new Candidate[0];
            }

            var allCandidates = DopeElectionsApp.Instance.Assets.GetAssets<Candidate>();

            return list.candidates
                .OrderBy(e => e.pos)
                .Select(e => allCandidates.FirstOrDefault(c => c.id == e.id))
                .Where(c => c != null).ToArray();
        }

        public static Color GetColor(this ElectionList list)
        {
            var assets = DopeElectionsApp.Instance.Assets;
            var candidates = list.GetCandidates();
            if (candidates.Length == 0) return Color.gray;
            
            var partyIds = candidates.Select(c => c.partyId);
            var parties = partyIds.Distinct()
                .Select(partyId => assets.GetAsset<Party>(partyId))
                .Where(p => p != null)
                .ToDictionary(p => p.id, p => p.GetColor());
            var colors = candidates
                .Select(c => parties.TryGetValue(c.partyId, out var color) ? color : Color.gray)
                .ToList();
            var r = colors.Sum(c => c.r) / colors.Count;
            var g = colors.Sum(c => c.g) / colors.Count;
            var b = colors.Sum(c => c.b) / colors.Count;
            return new Color(r, g, b);
        }

        public static SmartSpider GetSmartSpider(this ElectionList list)
        {
            return list.GetCandidates().GetSmartSpider();
        }

        public static LocalizationKey GetListNumberKey(this ElectionList list)
        {
            return list.number > 0
                ? !list.adapted
                    ? LKey.Components.ElectionList.ListNumber.Official
                    : LKey.Components.ElectionList.ListNumber.Modified
                : LKey.Components.ElectionList.ListNumber.Custom;
        }
    }
}