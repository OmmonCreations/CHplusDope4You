using DopeElections.Answer;
using DopeElections.Placeholders;
using UnityEngine;

namespace DopeElections.CandidateParties
{
    public static class CandidatePartyExtensions
    {
        public static WebSprite GetLogo(this Party party, Sprite fallback = null)
        {
            var logoPath = party.logo;
            return WebSprite.Load(logoPath, fallback);
        }

        public static Color GetColor(this Party party)
        {
            return !string.IsNullOrWhiteSpace(party.color) && ColorUtility.TryParseHtmlString(party.color, out var c)
                ? c
                : default;
        }
    }
}