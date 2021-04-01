using System.Collections.Generic;
using UnityEngine;

namespace Html
{
    public interface IHtmlElement
    {
        RectTransform RectTransform { get; }
        string Tag { get; }
        IHtmlElement[] ChildNodes { get; }
        
        void Initialize(HtmlCanvas canvas, IHtmlElement parent, string tag, IEnumerable<KeyValuePair<string, string>> attributes, string body);
        void Remove();

        void AppendChild(IHtmlElement element);
        void PrependChild(IHtmlElement element);
        void RemoveChild(IHtmlElement element);
        void InsertChild(int index, IHtmlElement element);

        void UpdateLayout();

        IHtmlElement Clone();
    }
}