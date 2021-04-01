using AnimatedObjects;
using DopeElections.Answer;
using DopeElections.Questions;
using Localizator;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DopeElections.RaceCategorySelections
{
    public class CategoryController : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private Button _button = null;
        [SerializeField] private Image _icon = null;
        [SerializeField] private LocalizedText _labelText = null;
        [SerializeField] private ToggleableObjectController _selectAnimationController = null;
        [SerializeField] private ToggleableObjectController _animationController = null;

        private bool _selected = false;

        public RectTransform RectTransform => _rectTransform;
        public UnityEvent onClick => _button.onClick;
        public QuestionCategory Category { get; private set; }
        public Vector2Int Position { get; private set; }

        public bool Selected
        {
            get => _selected;
            set => ApplySelected(value);
        }

        public void Initialize(QuestionCategory category, Vector2Int position)
        {
            Category = category;
            Position = position;
            _icon.sprite = category.GetIconWhite();
            _labelText.key = new LocalizationKey {fallback = category.name};
            _animationController.HideImmediate();
            SetSelectedImmediate(false);
        }

        public void Show(float delay = 0)
        {
            _animationController.Show(delay);
        }

        public void Remove(float delay = 0)
        {
            _animationController.Hide(delay).Then(() => Destroy(gameObject));
        }

        public void SetSelectedImmediate(bool selected)
        {
            _selected = selected;
            _selectAnimationController.ShowImmediate(selected);
        }

        public void UpdateLayout(Vector2 origin, Vector2 size, Vector2 spacing)
        {
            var rectTransform = RectTransform;
            var position = Position;
            rectTransform.anchorMin = Vector2.up;
            rectTransform.anchorMax = Vector2.up;
            rectTransform.pivot = Vector2.up;
            rectTransform.offsetMin = origin + new Vector2(
                position.x * size.x + position.x * spacing.x,
                -((position.y + 1) * size.y + position.y * spacing.y)
            );
            rectTransform.offsetMax = origin + new Vector2(
                (position.x + 1) * size.x + position.x * spacing.x,
                -(position.y * size.y + position.y * spacing.y)
            );
        }

        private void ApplySelected(bool selected)
        {
            _selected = selected;
            _selectAnimationController.Show(selected);
        }
    }
}