using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Html
{
    [CreateAssetMenu(fileName = "HtmlTagStyleMap", menuName = "Html/Html Tag Style Map")]
    public class HtmlTagStyleMap : ScriptableObject, IEnumerable<HtmlTagStyleMap.TagStyleEntry>
    {
        [SerializeField] private TagStyleEntry[] _entries = null;

        public IEnumerator<TagStyleEntry> GetEnumerator()
        {
            return _entries.GroupBy(e => e.name).Select(g => g.First()).Where(e => !string.IsNullOrWhiteSpace(e.name))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Serializable]
        public class TagStyleEntry
        {
            public string name;
            public string style;
            public bool linebreak;
            public float spacing;
        }
    }
}