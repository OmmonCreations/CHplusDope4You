using UnityEngine;
using UnityEngine.UI;

namespace Essentials
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class CircularGraphicTarget : Graphic
    {
        public override void Rebuild(CanvasUpdate update)
        {
        }

        public override bool Raycast(Vector2 sp, Camera eventCamera)
        {
            var rectTransform = this.rectTransform;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out var p))
            {
                return false;
            }

            var rect = rectTransform.rect;
            var relativePoint = (p - rect.min) / rect.size;

            var distanceFromCenter = ((relativePoint - new Vector2(0.5f, 0.5f)) * 2).sqrMagnitude;

            return distanceFromCenter <= 1;
        }
    }
}