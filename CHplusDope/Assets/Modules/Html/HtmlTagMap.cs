using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Html
{
    [CreateAssetMenu(fileName = "HtmlTagMap", menuName = "Html/Html Tag Map")]
    public class HtmlTagMap : ScriptableObject, IEnumerable<HtmlTagMap.TagEntry>
    {
        [SerializeField] private string[] _excludedTags = null;
        [SerializeField] private string[] _selfClosingTags = null;
        [SerializeField] private TagEntry[] _entries = null;

        public string[] ExcludedTags => _excludedTags;
        public string[] SelfClosingTags => _selfClosingTags;
        
        public IEnumerator<TagEntry> GetEnumerator()
        {
            return _entries.GroupBy(e => e.name).Select(g => g.First()).Where(e => !string.IsNullOrWhiteSpace(e.name))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Serializable]
        public class TagEntry
        {
            public string name;
            public HtmlElement prefab;
        }
    }
}