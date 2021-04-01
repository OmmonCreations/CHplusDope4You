using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Html
{
    public class HtmlTextElement : HtmlElement
    {
        private static readonly Regex TagRegex = new Regex("<([a-zA-Z0-9_-]+)(?:[^>]+)>");

        [SerializeField] private TMP_Text _text = null;

        protected override void GenerateBody(string body)
        {
            var styleMap = Canvas.StyleMap;

            var nestedTagMatches = TagRegex.Matches(body);
            var nestedTagFound = nestedTagMatches.Cast<Match>().Where(m =>
            {
                var nestedTagName = m.Success
                    ? m.Groups[1].Value
                    : null;
                return m.Success && Canvas.TagMap.ExcludedTags.All(t => t != nestedTagName);
            }).FirstOrDefault();
            if (nestedTagFound != null)
            {
                /*
                foreach (var child in Canvas.CreateElements(this, body))
                {
                    AppendChild(child);
                }*/

                Debug.LogWarning("Nested HTML tags are not supported. Found: " + nestedTagFound.Value + " in\n" + body);
            }
            else
            {
                var styleSheet = Canvas.StyleSheet;
                var html = $"<{Tag}>{body}</{Tag}>";
                var richText = HtmlUtility.ConvertHtmlToRichText(html, styleMap, styleSheet);
                _text.text = richText;
            }

            var style = styleMap.FirstOrDefault(s => s.name == Tag);
            if (style != null)
            {
                ApplyStyles(style);
            }
        }

        private void ApplyStyles(HtmlTagStyleMap.TagStyleEntry style)
        {
            if (style.spacing > 0)
            {
                _text.margin = new Vector4(0, 0, 0, style.spacing);
            }
        }
    }
}