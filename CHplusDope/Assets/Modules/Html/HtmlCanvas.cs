using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Html
{
    public class HtmlCanvas : UIBehaviour
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        private static readonly List<KeyValuePair<string, string>> EmptyAttributes =
            new List<KeyValuePair<string, string>>();

        [SerializeField] private HtmlTagMap _tagMap = null;
        [SerializeField] private TMP_StyleSheet _styleSheet = null;
        [SerializeField] private HtmlTagStyleMap _styleMap = null;

        private string _html;

        public TMP_StyleSheet StyleSheet => _styleSheet;
        public HtmlTagStyleMap StyleMap => _styleMap;
        public HtmlTagMap TagMap => _tagMap;

        private readonly List<IHtmlElement> _elements = new List<IHtmlElement>();

        public string Html
        {
            get => _html;
            set => ApplyHtml(value);
        }

        private void ApplyHtml(string html)
        {
            _html = html;
            ClearElements();
            if (html != null) _elements.AddRange(CreateElements(null, html));
        }

        private void ClearElements()
        {
            foreach (var e in _elements) e.Remove();
            _elements.Clear();
        }

        internal IEnumerable<IHtmlElement> CreateElements(IHtmlElement parent, string html)
        {
            var result = new List<IHtmlElement>();
            const string openingTagPattern =
                "^<([a-zA-Z0-9]+)([ ]*(?:[ ]*[a-zA-Z0-9_-]+(?:=(?:(?:\\\"[^\"]*\\\")|(?:\\\'[^']*\\\')))?)*)[ ]*(/)?>";
            var openingTagRegex = new Regex(openingTagPattern);
            var selfClosingTags = TagMap.SelfClosingTags;

            var iteration = 0;
            const int maxIterations = 1000;

            var originalHtml = html;
            var buffer = "";

            html = html.Replace("\n", " ");
            html = html.Replace("\t", " ");

            while (html.Length > 0 && iteration < maxIterations)
            {
                iteration++;
                var tagMatch = openingTagRegex.Match(html);
                var tag = tagMatch.Groups.Count > 1 ? tagMatch.Groups[1].Value : null;
                if (tag == null)
                {
                    buffer += html[0];
                    html = html.Substring(1);
                    continue;
                }

                if (TagMap.ExcludedTags.Any(t => t == tag))
                {
                    buffer += tagMatch.Value;
                    html = html.Substring(tagMatch.Value.Length);
                    continue;
                }

                var attributesString = tagMatch.Groups[2].Value;
                var attributes = ParseAttributes(attributesString);
                var isSelfClosing = selfClosingTags.Any(t => t == tag) ||
                                    tagMatch.Groups.Count > 3 && tagMatch.Groups[3].Value == "/";

                var tagLength = tagMatch.Groups[0].Value.Length;
                html = html.Substring(tagLength);

                IHtmlElement bufferedText = null;
                if (isSelfClosing)
                {
                    if (!string.IsNullOrWhiteSpace(buffer))
                    {
                        bufferedText = CreateElement(parent, "p", EmptyAttributes, buffer);
                        buffer = "";
                        if (bufferedText != null) result.Add(bufferedText);
                    }

                    var selfClosingElement = CreateElement(parent, tag, attributes, "");
                    if (selfClosingElement == null) continue;
                    result.Add(selfClosingElement);
                    continue;
                }

                var closingTagPattern = $"</{tag}>";
                var closingTagPosition = html.IndexOf(closingTagPattern, StringComparison.Ordinal);
                if (closingTagPosition < 0) closingTagPosition = html.Length;

                var body = html.Substring(0, closingTagPosition);
                var bodyWithEndingTagLength = closingTagPosition + closingTagPattern.Length;

                html = html.Length >= bodyWithEndingTagLength ? html.Substring(bodyWithEndingTagLength) : "";

                if (!string.IsNullOrWhiteSpace(buffer))
                {
                    bufferedText = CreateElement(parent, "p", EmptyAttributes, buffer);
                    buffer = "";
                    if (bufferedText != null) result.Add(bufferedText);
                }

                var element = CreateElement(parent, tag, attributes, body);
                if (element == null) continue;
                result.Add(element);
            }

            if (!string.IsNullOrWhiteSpace(buffer))
            {
                var remainingText = buffer;
                var textElement = CreateElement(parent, "p", EmptyAttributes, remainingText);
                if (textElement != null) result.Add(textElement);
                // Debug.Log("Created remaining text:\n" + remainingText);
            }

            if (iteration >= maxIterations)
            {
                Debug.LogWarning("Parsing of " + originalHtml + " is incomplete.\n" + result.Count +
                                 " elements generated");
            }

            return result;
        }

        public IEnumerable<KeyValuePair<string, string>> ParseAttributes(string attributesString)
        {
            var pattern = "(?:[ ]*([a-zA-Z0-9_-]+)=(?:(?:\\\"([^\"]*)\\\")|(?:\\\'([^']*)\\\')))";
            var regex = new Regex(pattern);
            var matches = regex.Matches(attributesString);
            var result = new List<KeyValuePair<string, string>>();
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var attributeName = match.Groups[1].Value;
                var attributeValue = match.Groups.Count > 0 ? match.Groups[2].Value : null;
                result.Add(new KeyValuePair<string, string>(attributeName, attributeValue));
            }

            return result;
        }

        public IHtmlElement CreateElement(IHtmlElement parent, string tag,
            IEnumerable<KeyValuePair<string, string>> attributes, string body)
        {
            // Debug.Log($"Create <b>{tag}</b>:\n{body}");
            var tagType = TagMap
                .Where(t => t.name == tag)
                .DefaultIfEmpty(TagMap.FirstOrDefault(t => t.name == "p"))
                .First();
            var result = tagType != null ? tagType.prefab.Clone() : null;
            if (result == null) return null;
            result.Initialize(this, parent, tag, attributes, body.Trim());
            if (parent == null) result.RectTransform.SetParent(transform, false);
            else parent.AppendChild(result);
            return result;
        }
    }
}