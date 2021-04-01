using System;
using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.CandidateParties;
using DopeElections.Placeholders;
using UnityEngine;

namespace DopeElections.Candidates
{
    public static class CandidateExtensions
    {
        public static Party GetParty(this Candidate candidate)
        {
            return DopeElectionsApp.Instance.Assets.GetAsset<Party>(p =>
                p.id == candidate.partyId);
        }

        public static Color GetPartyColor(this Candidate candidate)
        {
            var party = candidate.GetParty();
            return party != null ? party.GetColor() : Color.gray;
        }

        public static int GetAverageAge(this Candidate[] candidates)
        {
            var year = DateTime.Now.Year;
            return candidates.Length > 0
                ? (int) Math.Round(candidates.Select(c => c.birthYear).Average(y => year - y))
                : 0;
        }

        public static float GetFemaleRatio(this Candidate[] candidates)
        {
            return candidates.Length > 0 ? 1 - candidates.GetMaleRatio() : float.NaN;
        }

        public static float GetMaleRatio(this Candidate[] candidates)
        {
            return candidates.Length > 0
                ? (float) candidates.Count(c => c.gender == "m") / candidates.Count()
                : float.NaN;
        }

        public static float GetFemaleRatio(this List<Candidate> candidates)
        {
            return candidates.Count > 0 ? 1 - candidates.GetMaleRatio() : 0;
        }

        public static float GetMaleRatio(this List<Candidate> candidates)
        {
            return candidates.Count > 0 ? (float) candidates.Count(c => c.gender == "m") / candidates.Count() : 0;
        }

        public static Party[] GetParties(this IEnumerable<Candidate> candidates)
        {
            var assets = DopeElectionsApp.Instance.Assets;
            var partyIds = candidates.Select(c => c.partyId).Distinct().OrderBy(p => p).ToArray();
            return partyIds
                .Select(partyId => assets.GetAsset<Party>(p => p.id == partyId))
                .Where(p => p != null)
                .ToArray();
        }

        public static SmartSpider GetSmartSpider(this IEnumerable<Candidate> candidates)
        {
            var smartSpiders = candidates.Where(c => c.smartSpider.HasData).Select(c => c.smartSpider)
                .DefaultIfEmpty(new SmartSpider()).ToArray();
            return new SmartSpider()
            {
                axis_1 = smartSpiders.Average(s => s.axis_1),
                axis_2 = smartSpiders.Average(s => s.axis_2),
                axis_3 = smartSpiders.Average(s => s.axis_3),
                axis_4 = smartSpiders.Average(s => s.axis_4),
                axis_5 = smartSpiders.Average(s => s.axis_5),
                axis_6 = smartSpiders.Average(s => s.axis_6),
                axis_7 = smartSpiders.Average(s => s.axis_7),
                axis_8 = smartSpiders.Average(s => s.axis_8)
            };
        }

        public static WebSprite GetIcon(this Candidate candidate, Sprite placeholderSprite = null)
        {
            return WebSprite.Load(candidate.urlImage, WebSprite.GetTextureResampler(new Vector2Int(512, 512)),
                WebSprite.GetSquareViewport, placeholderSprite);
        }
    }
}