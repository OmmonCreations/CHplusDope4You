using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Layouts
{
    public class VerticalLayoutModifier : LayoutGroupModifier
    {
        public override void SetAlignment(TextAnchor anchor)
        {
            var layoutGroup = GetComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = anchor;
        }
    }
}