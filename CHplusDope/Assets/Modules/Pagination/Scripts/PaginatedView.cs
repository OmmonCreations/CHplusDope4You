using UnityEngine;

namespace Pagination
{
    public static class PaginatedView
    {
        public enum Direction
        {
            Right,
            Left,
            Up,
            Down,
        }

        public enum Alignment
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
        }

        public static Vector2 GetOrigin(Vector2 area, Direction primary, Direction secondary)
        {
            var leftSide = primary == Direction.Right || secondary == Direction.Right;
            var topSide = primary == Direction.Down || secondary == Direction.Down;
            return new Vector2(leftSide ? 0 : area.x, topSide ? 0 : -area.y);
        }

        public static Vector2 GetPivot(Direction primary, Direction secondary)
        {
            var leftSide = primary == Direction.Right || secondary == Direction.Right;
            var topSide = primary == Direction.Down || secondary == Direction.Down;
            return new Vector2(leftSide ? 0 : 1, topSide ? 1 : 0);
        }

        public static Vector2 GetStep(Vector2 itemSize, Vector2 spacing, Direction direction)
        {
            var size = itemSize + spacing;
            var rightToLeft = direction == Direction.Right;
            var horizontal = direction == Direction.Right || direction == Direction.Left;
            var topToBottom = direction == Direction.Down;
            var vertical = direction == Direction.Down || direction == Direction.Up;
            return new Vector2(
                horizontal
                    ? rightToLeft
                        ? size.x
                        : -size.x
                    : 0,
                vertical
                    ? topToBottom
                        ? -size.y
                        : size.y
                    : 0
            );
        }

        public static int GetCount(Vector2 area, Vector2 itemSize, Vector2 spacing, Direction direction)
        {
            var primaryHorizontal = direction == Direction.Left || direction == Direction.Right;
            var primaryAreaSize = primaryHorizontal ? area.x + spacing.x : area.y + spacing.y;
            var primaryItemSize = primaryHorizontal ? itemSize.x + spacing.x : itemSize.y + spacing.y;
            return Mathf.Max(1, Mathf.FloorToInt(primaryAreaSize / primaryItemSize));
        }

        public static int GetTotalCount(Vector2 area, Vector2 itemSize, Vector2 spacing, Direction primaryDirection,
            bool wrap)
        {
            var primaryCount = GetCount(area, itemSize, spacing, primaryDirection);
            if (!wrap) return primaryCount;
            var secondaryDirection = primaryDirection == Direction.Left || primaryDirection == Direction.Right
                ? Direction.Down
                : Direction.Right;
            var secondaryCount = GetCount(area, itemSize, spacing, secondaryDirection);
            return primaryCount * secondaryCount;
        }
    }
}