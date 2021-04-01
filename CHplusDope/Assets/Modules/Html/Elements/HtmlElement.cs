using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Html
{
    public abstract class HtmlElement : UIBehaviour, IHtmlElement
    {
        private static readonly Regex NumberRegex = new Regex("^(-?[0-9.]+)");

        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private LayoutGroup _layoutGroup = null;
        [SerializeField] private LayoutElement _layoutElement = null;

        protected HtmlCanvas Canvas { get; private set; }
        public IHtmlElement Parent { get; private set; }
        public RectTransform RectTransform => _rectTransform;
        public string Tag { get; private set; }

        private Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();
        private readonly List<IHtmlElement> _children = new List<IHtmlElement>();

        public IHtmlElement[] ChildNodes => _children.ToArray();

        public float Width
        {
            get => _layoutElement.preferredWidth;
            set => _layoutElement.preferredWidth = value;
        }

        public float Height
        {
            get => _layoutElement.preferredHeight;
            set => _layoutElement.preferredHeight = value;
        }

        public void Initialize(HtmlCanvas canvas, IHtmlElement parent, string tag,
            IEnumerable<KeyValuePair<string, string>> attributes, string body)
        {
            Canvas = canvas;
            Parent = parent;
            Tag = tag;
            foreach (var entry in attributes) Attributes[entry.Key] = entry.Value;
            GenerateBody(body);
            if (Attributes.TryGetValue("style", out var styles)) ApplyStyles(styles);
            
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }

        protected virtual void GenerateBody(string body)
        {
            foreach (var child in Canvas.CreateElements(this, body))
            {
                AppendChild(child);
            }
        }

        public string GetAttribute(string attribute)
        {
            return Attributes.TryGetValue(attribute, out var v) ? v : null;
        }

        public void SetAttribute(string attribute, string value)
        {
            Attributes[attribute] = value;
        }

        public void Remove()
        {
            Destroy(gameObject);
            OnRemove();
        }

        protected virtual void OnRemove()
        {
        }

        public void AppendChild(IHtmlElement element)
        {
            _children.Add(element);
            element.RectTransform.SetParent(RectTransform, false);
        }

        public void PrependChild(IHtmlElement element)
        {
            _children.Insert(0, element);
            element.RectTransform.SetParent(RectTransform, false);
            element.RectTransform.SetSiblingIndex(0);
        }

        public void InsertChild(int index, IHtmlElement element)
        {
            _children.Insert(index, element);
            element.RectTransform.SetParent(RectTransform, false);
            element.RectTransform.SetSiblingIndex(index);
        }

        public void RemoveChild(IHtmlElement element)
        {
            _children.Remove(element);
            element.RectTransform.parent = null;
        }

        public void UpdateLayout()
        {
            foreach (var child in _children) child.UpdateLayout();
        }

        public IHtmlElement Clone()
        {
            return Instantiate(this);
        }

        private void ApplyStyles(string stylesString)
        {
            var entries = stylesString.Split(';');
            foreach (var entry in entries)
            {
                var parts = entry.Split(':');
                if (parts.Length < 2) continue;
                var key = parts[0];
                var value = parts[1];
                ApplyStyle(key, value);
            }
        }

        private void ApplyStyle(string key, string valueString)
        {
            switch (key.ToLower())
            {
                case "margin":
                {
                    var values = valueString.Split(' ').Select(ParseValue).ToArray();
                    ApplyMargin(values);
                    break;
                }

                case "margin-top":
                {
                    var value = ParseValue(valueString);
                    ApplyMarginTop(value);
                    break;
                }

                case "margin-right":
                {
                    var value = ParseValue(valueString);
                    ApplyMarginRight(value);
                    break;
                }

                case "margin-bottom":
                {
                    var value = ParseValue(valueString);
                    ApplyMarginBottom(value);
                    break;
                }

                case "margin-left":
                {
                    var value = ParseValue(valueString);
                    ApplyMarginLeft(value);
                    break;
                }

                case "width":
                {
                    var value = ParseValue(valueString);
                    ApplyWidth(value);
                    break;
                }

                case "height":
                {
                    var value = ParseValue(valueString);
                    ApplyHeight(value);
                    break;
                }
            }
        }

        private void ApplyMargin(params float[] values)
        {
            switch (values.Length)
            {
                case 1:
                {
                    ApplyMarginTop(values[0]);
                    ApplyMarginRight(values[0]);
                    ApplyMarginBottom(values[0]);
                    ApplyMarginLeft(values[0]);
                    break;
                }
                case 2:
                {
                    ApplyMarginTop(values[0]);
                    ApplyMarginRight(values[1]);
                    ApplyMarginBottom(values[0]);
                    ApplyMarginLeft(values[1]);
                    break;
                }
                case 3:
                {
                    ApplyMarginTop(values[0]);
                    ApplyMarginRight(values[1]);
                    ApplyMarginBottom(values[2]);
                    ApplyMarginLeft(values[1]);
                    break;
                }
                case 4:
                {
                    ApplyMarginTop(values[0]);
                    ApplyMarginRight(values[1]);
                    ApplyMarginBottom(values[2]);
                    ApplyMarginLeft(values[3]);
                    break;
                }
            }
        }

        private void ApplyMarginTop(float value)
        {
            _layoutGroup.padding.top = Mathf.RoundToInt(value);
        }

        private void ApplyMarginRight(float value)
        {
            _layoutGroup.padding.right = Mathf.RoundToInt(value);
        }

        private void ApplyMarginBottom(float value)
        {
            _layoutGroup.padding.bottom = Mathf.RoundToInt(value);
        }

        private void ApplyMarginLeft(float value)
        {
            _layoutGroup.padding.left = Mathf.RoundToInt(value);
        }

        private void ApplyWidth(float width)
        {
            Width = width;
        }

        private void ApplyHeight(float height)
        {
            Height = height;
        }

        private float ParseValue(string value)
        {
            var sanitized = NumberRegex.Match(value).Groups[1].Value;
            return float.TryParse(sanitized, out var f) ? f : 0;
        }
    }
}