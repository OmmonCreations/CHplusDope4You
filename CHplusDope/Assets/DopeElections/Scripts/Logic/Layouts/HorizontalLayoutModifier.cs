using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Layouts
{
    public class HorizontalLayoutModifier : LayoutGroupModifier
    {
        public override void SetAlignment(TextAnchor anchor)
        {
            var layoutGroup = GetComponent<HorizontalLayoutGroup>();
            layoutGroup.childAlignment = anchor;
        }
    }
}