using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Html
{
    public static class HtmlUtility
    {
        public static string ConvertHtmlToRichText(string html, HtmlTagStyleMap styleMap, TMP_StyleSheet styleSheet)
        {
            var richText = html;
            foreach (var entry in styleMap)
            {
                var tag = entry.name;
                var style = styleSheet.GetStyle(entry.style);
                if (style == null)
                {
                    Debug.LogWarning($"Style {entry.style} for tag {tag} not found.");
                    continue;
                }

                var openingTagPattern =
                    $"<{tag}(?:[ ]*([a-zA-Z0-9_-]+(?:=(?:(?:\\\"[^\"]*\\\")|(?:\\\'[^']*\\\')))?)*)[ ]*/?>";
                var closingTagPattern = $"</{tag}>";
                var openingRegex = new Regex(openingTagPattern);
                var closingRegex = new Regex(closingTagPattern);

                var openingStyle = style.styleOpeningDefinition;
                var closingStyle = style.styleClosingDefinition;

                if (entry.linebreak)
                {
                    closingStyle += "\n";
                    if (entry.spacing > 0)
                    {
                        closingStyle += "<size=\"" + entry.spacing + "\"></size>\n";
                    }
                }

                richText = openingRegex.Replace(richText, openingStyle);
                richText = closingRegex.Replace(richText, closingStyle);
            }

            var lineBreakRegex = new Regex("<br ?/?>");
            richText = lineBreakRegex.Replace(richText, "\n");

            var obsoleteEndingLineBreakRegex = new Regex("[\n]+$");
            richText = obsoleteEndingLineBreakRegex.Replace(richText, "");

            return richText;
        }
    }
}