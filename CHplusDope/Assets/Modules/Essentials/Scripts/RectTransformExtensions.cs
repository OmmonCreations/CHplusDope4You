using System.Linq;
using UnityEngine;

namespace Essentials
{
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Given a normalized point in the RectTransform, returns a local position within the transform based on its
        /// width, height and pivot
        /// </summary>
        public static Vector2 GetLocalPosition(this RectTransform rectTransform, Vector2 normalizedPoint)
        {
            var rect = rectTransform.rect;
            var localPosition = rect.min + new Vector2(rect.width * normalizedPoint.x, rect.height * normalizedPoint.y);
            /*
            Debug.Log("Get " + normalizedPoint + " in rect " + rect + ":\n" +
                      "Pivot: " + pivotPosition + "\n" +
                      "Local: " + localPosition+"\n" +
                      "Result: "+(localPosition - pivotPosition));
                      */
            return localPosition;
        }

        /// <summary>
        /// Given a local point in the RectTransform, returns a normalized point relative to the RectTransform Rect
        /// </summary>
        public static Vector2 GetNormalizedPosition(this RectTransform rectTransform, Vector2 localPoint)
        {
            var rect = rectTransform.rect;
            var relativePoint = localPoint - rect.min;
            return new Vector2(relativePoint.x / rect.width, relativePoint.y / rect.height);
        }

        public static Vector2 TransformPoint(this RectTransform rectTransform, RectTransform target, Vector2 point)
        {
            var cornersA = new Vector3[4];
            var cornersB = new Vector3[4];

            rectTransform.GetWorldCorners(cornersA);
            target.GetWorldCorners(cornersB);

            var worldSizeA = cornersA[2] - cornersA[0];
            var worldSizeB = cornersB[2] - cornersB[0];

            // normalized point in local rectangle A
            var normalizedPointA = rectTransform.GetNormalizedPosition(point);
            // world point
            var worldPoint = cornersA[0] + new Vector3(
                worldSizeA.x * normalizedPointA.x,
                worldSizeA.y * normalizedPointA.y
            );

            // local point in world rectangle B
            var relativePointB = worldPoint - cornersB[0];
            // normalized point rectangle B
            var normalizedPointB = new Vector2(
                relativePointB.x / worldSizeB.x,
                relativePointB.y / worldSizeB.y
            );
            // local point in local rectangle B
            var anchoredPoint = target.GetLocalPosition(normalizedPointB);

            /*
            Debug.Log(
                "local point in local rectangle A: " + point + "\n" +
                "normalized point in rectangle A: " + normalizedPointA + "\n" +
                "world rect A: " + string.Join(", ", cornersA.Select(c=>c.ToString())) + "\n" +
                "world point: " + worldPoint + "\n" +
                "relative point in world rectangle B: " + relativePointB + "\n" +
                "normalized point in world rectangle B: " + normalizedPointB + "\n" +
                "result: " + anchoredPoint);
                */

            return anchoredPoint;
        }

        public static void Fill(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public static void Overlay(this RectTransform rectTransform, RectTransform other)
        {
            var rect = other.rect;
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            var canvasTransform = canvas.transform;

            var canvasPosition = canvasTransform.position;

            var min = other.TransformPoint(rect.min) + canvasPosition;
            var max = other.TransformPoint(rect.max) + canvasPosition;
            var canvasMin = canvasTransform.InverseTransformPoint(min);
            var canvasMax = canvasTransform.InverseTransformPoint(max);
            rectTransform.pivot = Vector2.one / 2;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.offsetMin = canvasMin;
            rectTransform.offsetMax = canvasMax;
        }
    }
}