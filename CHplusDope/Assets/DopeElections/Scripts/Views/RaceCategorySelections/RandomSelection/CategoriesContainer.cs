using DopeElections.Answer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DopeElections.RaceCategorySelections
{
    public class CategoriesContainer : MonoBehaviour
    {
        public delegate void SelectEvent(QuestionCategory category);

        public event SelectEvent CategorySelected = delegate { };

        [SerializeField] private LayoutElement _layoutElement = null;
        [SerializeField] private RectTransform _categoriesArea = null;
        [SerializeField] private CategoryController _template = null;
        [SerializeField] private Vector2 _entrySize = new Vector2(786.5f, 193f);
        [SerializeField] private Vector2 _spacing = new Vector2(24, 43);
        [SerializeField] private int _columns = 2;
        [SerializeField] private UnityEvent _onSelected = new UnityEvent();

        private QuestionCategory[] _categories;
        private QuestionCategory _selected;
        private CategoryController[] _entries = null;
        private Vector2Int _size;

        public QuestionCategory Selected
        {
            get => _selected;
            set => ApplySelected(value);
        }

        public QuestionCategory[] Categories
        {
            get => _categories;
            set => ApplyCategories(value);
        }

        private void Awake()
        {
            _template.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            UpdateLayout();
        }

        public void SetSelectedImmediate(QuestionCategory category)
        {
            _selected = category;

            if (_entries != null)
            {
                foreach (var entry in _entries)
                {
                    var selected = entry.Category == category;
                    if (entry.Selected == selected) continue;
                    entry.SetSelectedImmediate(selected);
                }
            }
        }

        private void ApplyCategories(QuestionCategory[] categories)
        {
            _categories = categories;
            var hadEntries = _entries != null;
            ClearEntries();
            _entries = CreateEntries(categories, hadEntries ? 0.2f : 0);

            UpdateLayout();
        }

        private void ApplySelected(QuestionCategory category)
        {
            _selected = category;

            if (_entries != null)
            {
                foreach (var entry in _entries)
                {
                    var selected = entry.Category == category;
                    if (entry.Selected == selected) continue;
                    entry.Selected = selected;
                }
            }
        }

        private void ClearEntries()
        {
            const float interval = 0.05f;
            if (_entries == null) return;
            for (var i = 0; i < _entries.Length; i++)
            {
                var e = _entries[i];
                e.Remove(i * interval);
            }
        }

        private CategoryController[] CreateEntries(QuestionCategory[] categories, float delay)
        {
            const float interval = 0.05f;
            var entries = new CategoryController[categories.Length];
            var columns = _columns;
            var rows = columns > 0 ? Mathf.CeilToInt(categories.Length / (float) columns) : 0;

            var width = columns * _entrySize.x + Mathf.Max(0, columns - 1) * _spacing.x;
            var height = rows * _entrySize.y + Mathf.Max(0, rows - 1) * _spacing.y;

            for (var i = 0; i < categories.Length; i++)
            {
                var row = Mathf.FloorToInt(i / (float) columns);
                var column = i - row * columns;
                var entry = CreateEntry(categories[i], new Vector2Int(column, row));
                entry.Show(i * interval + delay);
                entries[i] = entry;
            }

            _layoutElement.preferredWidth = width;
            _layoutElement.preferredHeight = height;
            _layoutElement.minHeight = height;
            _size = new Vector2Int(columns, rows);

            return entries;
        }

        private CategoryController CreateEntry(QuestionCategory category, Vector2Int position)
        {
            var instanceObject = Instantiate(_template.gameObject, _categoriesArea, false);
            var instance = instanceObject.GetComponent<CategoryController>();
            instance.Initialize(category, position);
            instance.SetSelectedImmediate(instance.Category == Selected);
            instance.onClick.AddListener(() =>
            {
                _onSelected.Invoke();
                CategorySelected(category);
            });
            instanceObject.SetActive(true);
            return instance;
        }

        private void UpdateLayout()
        {
            if (_entries == null) return;

            var rectTransform = _categoriesArea;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            // var calculatedSize = new Vector2(
            //     _size.x * _entrySize.x + Mathf.Max(0, _size.x - 1) * _spacing.x,
            //     _size.y * _entrySize.y + Mathf.Max(0, _size.y - 1) * _spacing.y
            // );
            // var areaSize = rectTransform.rect.size;
            // var origin = (areaSize - calculatedSize) / 2;
            // origin.y *= -1;

            foreach (var entry in _entries)
            {
                entry.UpdateLayout(Vector2.zero, _entrySize, _spacing);
            }
        }
    }
}