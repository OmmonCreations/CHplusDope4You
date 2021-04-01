using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using Localizator;
using MobileInputs;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Candidates
{
    /// <summary>
    /// Represents a sortable view of candidates where candidates are optionally separated in sections
    /// </summary>
    public class CandidateCollectionController : MonoBehaviour, ISlotContainer
    {
        public delegate void CandidateEvent(CandidateSlotController slot);

        public event CandidateEvent CandidateTapped = delegate { };
        public event CandidateEvent CandidateDragged = delegate { };

        [Header("Prefab References")]
        [SerializeField] private RectTransform _slotsArea = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private Button _filtersButton = null;
        [SerializeField] private LocalizedText _filtersText = null;
        [SerializeField] private Button _searchButton = null;
        [SerializeField] private NameFilterController _nameFilter = null;
        [SerializeField] private RectTransform _toolsArea = null;
        [SerializeField] private RectTransform _scrollArea = null;
        [SerializeField] private float _buttonsHeight = 100;
        [SerializeField] private float _nameInputHeight = 100;
        [SerializeField] private float _sectionHeaderHeight = 100;
        [SerializeField] private float _slotSize = 100;
        [SerializeField] private InteractionSystem _interactionSystem = null;
        [SerializeField] private LocalizedText _sectionHeaderPrefab = null;
        [SerializeField] private CandidateSlotController _slotPrefab = null;
        [Header("Scene References")]
        [SerializeField] private CandidateFiltersController _filters = null;

        private bool _useNameFilter = false;
        private CandidateCollection _collection;
        private CandidateSlotController[] _slots;
        private CandidateSection[] _sections;
        private LocalizedText[] _sectionHeaders;
        private bool _slotsPlaced = false;

        private bool _dragHorizontal;
        private bool _dragVertical;

        public float SlotSize => _slotSize;

        public CandidateCollection Collection
        {
            get => _collection;
            set => ApplyCollection(value);
        }

        public bool PreventScrolling
        {
            get => !_scrollRect.enabled;
            set
            {
                _scrollRect.enabled = !value;
                _scrollRect.velocity = Vector2.zero;
            }
        }

        public bool DragHorizontal
        {
            get => _dragHorizontal;
            set => ApplyDragHorizontal(value);
        }

        public bool DragVertical
        {
            get => _dragVertical;
            set => ApplyDragVertical(value);
        }

        public InteractionSystem InteractionSystem => _interactionSystem;

        private void Awake()
        {
            _filtersButton.onClick.AddListener(OpenFilters);
            _filtersText.key = LKey.Components.CandidateCollection.Filters.Label;
            _searchButton.onClick.AddListener(ToggleSearch);
            ShowSearch(false);
            _filters.CloseImmediately();
            ElectionList.ListUpdated += OnElectionListUpdated;
        }

        private void OnEnable()
        {
            _interactionSystem.enabled = true;
        }

        private void OnDisable()
        {
            _interactionSystem.enabled = false;
        }

        private void OnDestroy()
        {
            ElectionList.ListUpdated -= OnElectionListUpdated;
        }

        private void Update()
        {
            if (Collection != null) Collection.Update();
        }

        private void OpenFilters()
        {
            _filters.Open();
        }

        private void ToggleSearch()
        {
            ShowSearch(!_useNameFilter);
        }

        private void ShowSearch(bool show)
        {
            _useNameFilter = show;
            _nameFilter.gameObject.SetActive(show);
            if (!show && _nameFilter.Filter != null) _nameFilter.Filter.State = new NameFilter.FilterState();
            var toolsHeight = _buttonsHeight + (show ? _nameInputHeight : 0);
            _toolsArea.offsetMin = new Vector2(0, -toolsHeight);
            _scrollArea.offsetMax = new Vector2(0, -toolsHeight);
        }

        private void ApplyCollection(CandidateCollection collection)
        {
            _collection = collection;
            _filters.Collection = collection;
            _nameFilter.Initialize(collection, collection.Modifiers.OfType<NameFilter>().FirstOrDefault());
            ClearSlots();
            _slots = CreateSlots(collection.AllEntries).ToArray();
            collection.Updated += OnCollectionUpdated;
            collection.Update();
            OnCollectionUpdated();
        }

        private void ApplyDragHorizontal(bool drag)
        {
            _dragHorizontal = drag;
            if (_slots != null)
            {
                foreach (var s in _slots) s.DragHorizontal = drag;
            }
        }

        private void ApplyDragVertical(bool drag)
        {
            _dragVertical = drag;
            if (_slots != null)
            {
                foreach (var s in _slots) s.DragVertical = drag;
            }
        }

        private void OnElectionListUpdated(ElectionList list)
        {
            if (_slots == null) return;
            foreach (var s in _slots) s.UpdateFrame();
        }

        private void OnSlotTapped(CandidateSlotController slot)
        {
            CandidateTapped(slot);
        }

        private void OnSlotDragged(CandidateSlotController slot)
        {
            CandidateDragged(slot);
        }

        private void OnCollectionUpdated()
        {
            ClearSectionHeaders();
            var sections = Collection.GetCandidatesAsSections();
            _sectionHeaders = CreateSections(sections).ToArray();
            _sections = sections;
            UpdateLayout();
        }

        public void UpdateLayout()
        {
            if (_sections == null || _slots == null) return;

            var rect = _slotsArea.rect;
            var width = rect.width;
            var columnCount = Mathf.FloorToInt(width / SlotSize);
            var sections = _sections;
            var headers = _sectionHeaders;
            var slotMap =
                sections.ToDictionary(s => s,
                    s => _slots.Where(slot => s.Candidates.Contains(slot.Candidate)).ToArray());
            var hiddenSlots = _slots.Except(slotMap.SelectMany(e => e.Value));
            var y = 0f;
            var yOffset = 0f;
            var row = 0;
            for (var i = 0; i < sections.Length; i++)
            {
                var section = sections[i];
                var slots = slotMap[section];
                var header = headers[i];

                header.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -y);
                y += _sectionHeaderHeight;
                yOffset += _sectionHeaderHeight;
                var column = 0;
                foreach (var s in slots)
                {
                    if (!s.gameObject.activeSelf) s.gameObject.SetActive(true);
                    var slotPosition = new Vector2Int(column, row);
                    var position = CandidateSlotController.GetSlotVector(slotPosition, SlotSize) +
                                   new Vector2(0, -yOffset);
                    if (_slotsPlaced) s.SetPosition(slotPosition, position);
                    else s.SetPositionImmediately(slotPosition, position);
                    column++;
                    if (column >= columnCount)
                    {
                        column = 0;
                        row++;
                        y += SlotSize;
                    }
                }

                if (column > 0)
                {
                    row++;
                    y += SlotSize;
                }
            }

            foreach (var s in hiddenSlots) s.gameObject.SetActive(false);

            _slotsPlaced = true;

            _slotsArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
        }

        public Vector2 GetSlotVector(Vector2Int slotPosition) =>
            CandidateSlotController.GetSlotVector(slotPosition, SlotSize);

        private void ClearSlots()
        {
            if (_slots == null) return;
            foreach (var s in _slots)
            {
                s.Remove();
            }
        }

        private void ClearSectionHeaders()
        {
            if (_sectionHeaders == null) return;

            foreach (var s in _sectionHeaders)
            {
                Destroy(s.gameObject);
            }

            _sectionHeaders = null;
        }

        private IEnumerable<CandidateSlotController> CreateSlots(IEnumerable<Candidate> candidates)
        {
            return candidates.Select(CreateSlot);
        }

        private CandidateSlotController CreateSlot(Candidate candidate)
        {
            var instanceObject = Instantiate(_slotPrefab.gameObject, _slotsArea, false);
            var instance = instanceObject.GetComponent<CandidateSlotController>();
            instance.Initialize(this, 0);
            instance.ColumnCount = 0;
            instance.Candidate = candidate;
            instance.Tapped += () => OnSlotTapped(instance);
            instance.Dragged += () => OnSlotDragged(instance);
            instance.DragHorizontal = DragHorizontal;
            instance.DragVertical = DragVertical;
            return instance;
        }

        private IEnumerable<LocalizedText> CreateSections(IEnumerable<CandidateSection> sections)
        {
            return sections.Select(s => CreateSection(s));
        }

        private LocalizedText CreateSection(CandidateSection section)
        {
            var instanceObject = Instantiate(_sectionHeaderPrefab.gameObject, _slotsArea, false);
            var instance = instanceObject.GetComponent<LocalizedText>();
            instance.key = section.Title;
            return instance;
        }
    }
}