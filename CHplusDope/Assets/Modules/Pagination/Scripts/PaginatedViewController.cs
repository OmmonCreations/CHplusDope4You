using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pagination
{
    public class PaginatedViewController : UIBehaviour
    {
        #region Serialized Fields

        [Header("References")] [SerializeField]
        private RectTransform _entriesArea = null;

        [SerializeField] private PaginatedViewEntryController _entryTemplate = null;
        [SerializeField] private RectTransform _pageIndexesArea = null;
        [SerializeField] private PageIndexController _indexTemplate = null;

        [Header("Layout")] [Tooltip("Primary fill direction of items")] [SerializeField]
        private PaginatedView.Direction _primaryDirection = PaginatedView.Direction.Right;

        [Tooltip("Enable or disable multi line / column layouts")] [SerializeField]
        private bool _wrap = true;

        [Tooltip("Secondary fill direction of items (requires wrapping enabled)")] [SerializeField]
        private PaginatedView.Direction _secondaryDirection = PaginatedView.Direction.Down;

        // [Tooltip("Attachment point for item cluster")] [SerializeField]
        // private PaginatedView.Alignment _alignment = PaginatedView.Alignment.TopLeft;

        [SerializeField] private Vector2 _minItemSize = new Vector2(100, 100);
        [SerializeField] private Vector2 _spacing = new Vector2(0, 0);
        // [SerializeField] private bool _flexSize = true;
        // [SerializeField] private bool _flexSpacing = false;
        // [SerializeField] private bool _expandShort = false;

        #endregion

        #region Private Fields

        private PaginatedViewEntry[] _entries;
        private readonly List<PaginatedViewEntryController> _itemControllers = new List<PaginatedViewEntryController>();
        private readonly List<PageIndexController> _indexControllers = new List<PageIndexController>();
        private int _itemsPerPage;
        private int _currentPage;
        private bool _dirty;

        #endregion

        #region Properties

        public RectTransform EntriesArea => _entriesArea;
        public RectTransform PageIndexesArea => _pageIndexesArea;

        public PaginatedViewEntry[] Entries
        {
            get => _entries;
            set => ApplyEntries(value);
        }

        public PaginatedView.Direction PrimaryDirection
        {
            get => _primaryDirection;
            set => ApplyPrimaryDirection(value);
        }

        public PaginatedView.Direction SecondaryDirection
        {
            get => _secondaryDirection;
            set => ApplySecondaryDirection(value);
        }

        public bool Wrap
        {
            get => _wrap;
            set => ApplyWrap(value);
        }

        #endregion

        #region Unity Control

        protected override void Awake()
        {
            base.Awake();
            _entryTemplate.gameObject.SetActive(false);
            _indexTemplate.gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _dirty = true;
        }

        private void LateUpdate()
        {
            if (_dirty) UpdateView();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateView();
        }

        #endregion

        #region Public API

        public void RecalculateLayoutImmediate()
        {
            UpdateView();
        }

        public void SetLayoutDirty()
        {
            _dirty = true;
        }

        public void SetPage(int page)
        {
            _currentPage = page;
            foreach (var index in _indexControllers) index.SetActive(index.Index == page);
            UpdateContents();
        }

        public void Clear()
        {
            Entries = null;
            foreach (var e in _itemControllers) e.Remove();
            _itemControllers.Clear();
            foreach (var e in _indexControllers) e.Remove();
            _indexControllers.Clear();
        }

        #endregion

        #region Data Modifiers

        private void ApplyEntries(PaginatedViewEntry[] entries)
        {
            _entries = entries;
            _currentPage = 0;
            SetLayoutDirty();
        }

        private void ApplyPrimaryDirection(PaginatedView.Direction direction)
        {
            _primaryDirection = direction;
            SetLayoutDirty();
        }

        private void ApplySecondaryDirection(PaginatedView.Direction direction)
        {
            _secondaryDirection = direction;
            SetLayoutDirty();
        }

        private void ApplyWrap(bool wrap)
        {
            _wrap = wrap;
            SetLayoutDirty();
        }

        #endregion

        #region Logic

        private void UpdateView()
        {
            _dirty = false;

            var entries = Entries;
            if (entries == null)
            {
                Clear();
                return;
            }

            var entriesArea = _entriesArea;
            var indexesArea = _pageIndexesArea;
            var entriesAreaObject = entriesArea.gameObject;
            var indexesAreaObject = indexesArea.gameObject;
            LayoutRebuilder.ForceRebuildLayoutImmediate(entriesArea);

            var rect = EntriesArea.rect;
            var area = rect.size;

            var itemSize = _minItemSize;
            var spacing = _spacing;
            var itemsPerPage = Mathf.Min(
                entries.Length,
                PaginatedView.GetTotalCount(area, itemSize, spacing, PrimaryDirection, Wrap)
            );
            var pageCount = Mathf.CeilToInt(entries.Length / (float) itemsPerPage);

            var entriesAreaWasActive = entriesAreaObject.activeSelf;
            var indexesAreaWasActive = indexesAreaObject.activeSelf;
            if (entriesAreaWasActive) entriesAreaObject.SetActive(false);
            if (indexesAreaWasActive) indexesAreaObject.SetActive(false);

            SetEntryCount(itemsPerPage);
            UpdateLayout();
            UpdateContents();
            SetPageCount(pageCount);

            if (entriesAreaWasActive) entriesAreaObject.SetActive(true);
            if (indexesAreaWasActive) indexesAreaObject.SetActive(true);
        }

        private void UpdateLayout()
        {
            var rect = EntriesArea.rect;
            var area = rect.size;
            var itemSize = _minItemSize;
            var spacing = _spacing;
            var primaryDirection = _primaryDirection;
            var secondaryDirection = _secondaryDirection;
            var controllers = _itemControllers;
            var itemCount = controllers.Count;

            var origin = PaginatedView.GetOrigin(area, primaryDirection, secondaryDirection);
            var pivot = PaginatedView.GetPivot(primaryDirection, secondaryDirection);
            var primaryStep = PaginatedView.GetStep(itemSize, spacing, primaryDirection);
            var secondaryStep = PaginatedView.GetStep(itemSize, spacing, secondaryDirection);
            var primaryCount = PaginatedView.GetCount(area, itemSize, spacing, primaryDirection);
            var secondaryCount = PaginatedView.GetCount(area, itemSize, spacing, secondaryDirection);

            for (var j = 0; j < secondaryCount; j++)
            {
                for (var i = 0; i < primaryCount; i++)
                {
                    var index = j * primaryCount + i;
                    if (index >= itemCount) break;
                    var controller = _itemControllers[index];
                    var rectTransform = controller.rectTransform;
                    var relativePosition = i * primaryStep + j * secondaryStep;
                    rectTransform.pivot = pivot;
                    rectTransform.anchoredPosition = origin + relativePosition;
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSize.x);
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize.y);
                }
            }
        }

        private void UpdateContents()
        {
            var controllers = _itemControllers;
            var entries = _entries;
            var startIndex = _currentPage * _itemsPerPage;
            for (var i = 0; i < controllers.Count; i++)
            {
                var controller = controllers[i];
                var index = startIndex + i;
                controller.Entry = index < entries.Length ? entries[index] : null;
                controller.gameObject.SetActive(controller.Entry != null);
            }
        }

        private void SetEntryCount(int count)
        {
            _itemsPerPage = count;

            var controllers = _itemControllers;
            if (controllers.Count == count) return;

            var obsoleteCount = Mathf.Max(0, controllers.Count - count);
            var missingCount = Mathf.Max(0, count - controllers.Count);

            // remove obsolete controllers
            for (var i = 0; i < obsoleteCount; i++)
            {
                var index = controllers.Count - 1;
                var controller = controllers[index];
                controller.Remove();
                controllers.RemoveAt(index);
            }

            // create missing controllers
            for (var i = 0; i < missingCount; i++)
            {
                var controller = CreateEntry();
                controllers.Add(controller);
            }
        }

        private void SetPageCount(int count)
        {
            var controllers = _indexControllers;
            if (controllers.Count == count) return;
            
            var obsoleteCount = Mathf.Max(0, controllers.Count - count);
            var missingCount = Mathf.Max(0, count - controllers.Count);
            
            // remove obsolete controllers
            for (var i = 0; i < obsoleteCount; i++)
            {
                var index = controllers.Count - 1;
                var controller = controllers[index];
                controller.Remove();
                controllers.RemoveAt(index);
            }

            // create missing controllers
            for (var i = 0; i < missingCount; i++)
            {
                var controller = CreateIndex();
                controllers.Add(controller);
            }

            var currentPage = _currentPage >= 0 && _currentPage < controllers.Count ? _currentPage : 0;

            // update state
            for (var i = 0; i < controllers.Count; i++)
            {
                var controller = controllers[i];
                controller.Label = (i + 1).ToString();
                controller.Index = i;
                controller.SetActive(i == currentPage);
                controller.gameObject.SetActive(true);
            }

            _pageIndexesArea.gameObject.SetActive(controllers.Count > 1);
        }

        private PaginatedViewEntryController CreateEntry()
        {
            var instanceObject = Instantiate(_entryTemplate.gameObject, EntriesArea, false);
            if (instanceObject.activeSelf) instanceObject.SetActive(false);
            var instance = instanceObject.GetComponent<PaginatedViewEntryController>();
            instance.Initialize();
            var rectTransform = instance.rectTransform;
            rectTransform.anchorMin = Vector2.up;
            rectTransform.anchorMax = Vector2.up;
            return instance;
        }

        private PageIndexController CreateIndex()
        {
            var instanceObject = Instantiate(_indexTemplate.gameObject, PageIndexesArea, false);
            if (instanceObject.activeSelf) instanceObject.SetActive(false);
            var instance = instanceObject.GetComponent<PageIndexController>();
            instance.onClick.AddListener(() => SetPage(instance.Index));
            return instance;
        }

        #endregion
    }
}