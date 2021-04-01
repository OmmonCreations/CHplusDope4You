using System.Linq;
using System.Text.RegularExpressions;
using DopeElections.Answer;
using RuntimeAssetImporter;
using UnityEngine;

namespace DopeElections.Questions
{
    public static class QuestionCategoryExtensions
    {
        private static Regex SmartvoteIdRegex = new Regex("([0-9]+)\\.svg$");
    
        public static Sprite GetIconWhite(this QuestionCategory category)
        {
            var smartvoteIdString = GetSmartvoteIdString(category.image);
            var key = "question-category-" + smartvoteIdString;
            var asset = DopeElectionsApp.Instance.Assets.GetAssets<SpriteAsset>()
                .FirstOrDefault(a => a.Sprite.name == key);
            return asset != null ? asset.Sprite : null;
        }

        public static Sprite GetIconOutline(this QuestionCategory category)
        {
            var smartvoteIdString = GetSmartvoteIdString(category.image);
            var key = "question-category-user-" + smartvoteIdString;
            var asset = DopeElectionsApp.Instance.Assets.GetAssets<SpriteAsset>()
                .FirstOrDefault(a => a.Sprite.name == key);
            return asset != null ? asset.Sprite : null;
        }

        private static string GetSmartvoteIdString(string url)
        {
            if (url == null) return "";
            var match = SmartvoteIdRegex.Match(url);
            return match.Success ? match.Groups[1].Value : "";
        }
    }
}