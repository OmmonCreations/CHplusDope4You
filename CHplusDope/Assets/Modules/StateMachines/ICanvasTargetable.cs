using UnityEngine;
using Essentials;

namespace StateMachines
{
    public interface ICanvasTargetable : ITargetable
    {
        RectTransform RectTransform { get; }
    }

    public static class CanvasTargetableExtensions
    {
        /// <summary>
        /// Calculates the corresponding position of the pivot of the targetable inside a given RectTransform, use this to translate positions across different parents inside the canvas.
        /// </summary>
        /// <param name="targetable">The targetable of which you want to know the position</param>
        /// <param name="area">The area in which the resulting position should be calculated in</param>
        /// <returns>The relative x,y position inside the area, with x and y values between 0.0 (inclusive) and 1.0 (inclusive)</returns>
        public static Vector2 GetPosition(this ICanvasTargetable targetable, RectTransform area)
        {
            return targetable.GetPosition(area, targetable.RectTransform.pivot);
        }

        /// <summary>
        /// Calculates the corresponding position of a given anchor position of the targetable inside a given RectTransform, use this to translate positions across different parents inside the canvas.
        /// </summary>
        /// <param name="targetable"></param>
        /// <param name="area"></param>
        /// <param name="anchor"></param>
        /// <returns>The relative x,y position inside the area, with x and y values between 0.0 (inclusive) and 1.0 (inclusive)</returns>
        public static Vector2 GetPosition(this ICanvasTargetable targetable, RectTransform area, Vector2 anchor)
        {
            var rectTransform = targetable.RectTransform;
            var localPosition = rectTransform.GetLocalPosition(anchor);
            var result = rectTransform.TransformPoint(area, localPosition);
            // Debug.Log("Transformed " + localPosition + " to " + area.gameObject.name + ": " + result);
            return result;
        }
    }
}