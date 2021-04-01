using UnityEngine;

namespace DopeElections.Layouts
{
    public abstract class LayoutGroupModifier : MonoBehaviour
    {
        public void SetUpperLeftAlignment() => SetAlignment(TextAnchor.UpperLeft);
        public void SetMiddleLeftAlignment() => SetAlignment(TextAnchor.MiddleLeft);
        public void SetLowerLeftAlignment() => SetAlignment(TextAnchor.LowerLeft);
        
        public void SetUpperCenterAlignment() => SetAlignment(TextAnchor.UpperCenter);
        public void SetMiddleCenterAlignment() => SetAlignment(TextAnchor.MiddleCenter);
        public void SetLowerCenterAlignment() => SetAlignment(TextAnchor.LowerCenter);
        
        public void SetUpperRightAlignment() => SetAlignment(TextAnchor.UpperRight);
        public void SetMiddleRightAlignment() => SetAlignment(TextAnchor.MiddleRight);
        public void SetLowerRightAlignment() => SetAlignment(TextAnchor.LowerRight);

        public abstract void SetAlignment(TextAnchor anchor);
    }
}