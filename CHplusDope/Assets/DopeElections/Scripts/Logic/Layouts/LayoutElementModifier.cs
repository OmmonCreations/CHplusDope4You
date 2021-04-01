using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Layouts
{
    public class LayoutElementModifier : MonoBehaviour
    {
        public void SetMinWidth(float width)
        {
            var layoutElement = GetComponent<LayoutElement>();
            layoutElement.minWidth = width;
        }

        public void SetMinHeight(float height)
        {
            var layoutElement = GetComponent<LayoutElement>();
            layoutElement.minHeight = height;
        }

        public void SetPreferredWidth(float width)
        {
            var layoutElement = GetComponent<LayoutElement>();
            layoutElement.preferredWidth = width;
        }

        public void SetPreferredHeight(float height)
        {
            var layoutElement = GetComponent<LayoutElement>();
            layoutElement.preferredHeight = height;
        }

        public void SetFlexibleWidth(float width)
        {
            var layoutElement = GetComponent<LayoutElement>();
            layoutElement.flexibleWidth = width;
        }

        public void SetFlexibleHeight(float height)
        {
            var layoutElement = GetComponent<LayoutElement>();
            layoutElement.flexibleHeight = height;
        }
    }
}