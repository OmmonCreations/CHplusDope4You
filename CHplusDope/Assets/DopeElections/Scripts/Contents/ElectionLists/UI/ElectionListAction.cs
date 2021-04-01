using System;
using DopeElections.Answer;
using UnityEngine;

namespace DopeElections.ElectionLists.UI
{
    public class ElectionListAction
    {
        public Sprite Icon { get; }
        public Color Color { get; }
        public Action<ElectionList> Callback { get; }

        public ElectionListAction(Sprite icon, Action<ElectionList> callback) : this(icon, default, callback)
        {
        }

        public ElectionListAction(Sprite icon, Color color, Action<ElectionList> callback)
        {
            Icon = icon;
            Color = color;
            Callback = callback;
        }
    }
}