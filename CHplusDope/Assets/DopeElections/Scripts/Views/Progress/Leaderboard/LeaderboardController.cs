using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Candidates;
using DopeElections.Users;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Progress
{
    public class LeaderboardController : MonoBehaviour
    {
        public delegate void SelectEvent(Candidate candidate);

        public delegate void Event();

        public event SelectEvent Selected = delegate { };
        public event Event CandidatesChanged = delegate { };
        public event Event Scrolled = delegate { };

        [SerializeField] private LeaderboardEntryController _template = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private RectTransform _contentRect = null;
        [SerializeField] private RectTransform _entriesArea = null;
        [SerializeField] private float _entryHeight = 100;
        [SerializeField] private GraphicRaycaster _raycaster = null;
        
        [Header("Scene References")]
        [SerializeField] private Transform _cameraTarget = null;
        [SerializeField] private LeaderboardControls _controls = null;

        public CandidateCollection Collection { get; private set; }
        private Dictionary<Candidate, int> Ranks { get; } = new Dictionary<Candidate, int>();
        private float _fullHeight;
        private float _normalizedScrollPosition;

        private int _visibleEntries;
        private int _maxEntries;

        private readonly Queue<LeaderboardEntryController> _cache = new Queue<LeaderboardEntryController>();
        private readonly List<LeaderboardEntryController> _entries = new List<LeaderboardEntryController>();

        private float EntryHeight => _entryHeight;

        public int ScrollIndex
        {
            get => Mathf.RoundToInt(_normalizedScrollPosition *
                                    Mathf.Max(1, Collection.FilteredCount - _visibleEntries + 1));
            set => ApplyScrollIndex(value);
        }

        public int Length => Collection.FilteredCount;

        public CandidateCollection CandidateCollection
        {
            get => Collection;
            set => ApplyCandidates(value);
        }

        public bool Interactable
        {
            get => _raycaster.enabled;
            set => _raycaster.enabled = value;
        }

        public Transform CameraTarget => _cameraTarget;

        public void Initialize()
        {
            _scrollRect.onValueChanged.AddListener(OnScrolled);
            _template.gameObject.SetActive(false);
            _controls.Initialize(this);
        }

        public void UpdateCollection()
        {
            var user = DopeElectionsApp.Instance.User;
            var candidates = user.GetAvailableCandidates().ToArray();
            foreach (var c in candidates)
            {
                c.RecalculateMatch();
            }

            CandidateCollection = new CandidateCollection(candidates);
        }

        public void UpdateControls()
        {
            _controls.UpdateControls();
        }

        public void UpdateRanking()
        {
            ApplyCandidates(Collection);
        }

        private void OnScrolled(Vector2 position)
        {
            if (_fullHeight == 0) return;
            _normalizedScrollPosition = 1 - position.y;
            UpdateState();
            Scrolled();
        }

        private void OnEntrySelected(LeaderboardEntryController entry)
        {
            Selected(entry.Candidate);
        }

        private void ApplyCandidates(CandidateCollection collection)
        {
            if (Collection != null) Collection.Updated -= OnCollectionUpdated;

            foreach (var c in collection.AllEntries) c.RecalculateMatch();
            var matchSortingOrder = collection.Modifiers.OfType<MatchSortingOrder>().FirstOrDefault();
            if (matchSortingOrder != null)
            {
                matchSortingOrder.State = new MatchSortingOrder.FilterState(false);
                collection.SortingOrder = matchSortingOrder.Id;
            }

            Collection = collection;
            collection.Updated += OnCollectionUpdated;

            Ranks.Clear();
            var sortedCandidates = collection.AllEntries.OrderByDescending(c => c.match).ToList();
            for (var i = 0; i < sortedCandidates.Count; i++)
            {
                var c = sortedCandidates[i];
                Ranks[c] = i;
            }

            collection.Update();
        }

        private void OnCollectionUpdated()
        {
            _scrollRect.normalizedPosition = Vector2.up;

            UpdateArea();
            RecalculateMaxEntries();
            foreach (var entry in _entries) entry.Index = -1;
            UpdateState();
            CandidatesChanged();
        }

        private void ApplyScrollIndex(int index)
        {
            var scrollLength = Mathf.Max(1, Collection.FilteredCount - _visibleEntries + 1);
            var normalizedScrollPosition = index / (float) scrollLength;
            _scrollRect.verticalNormalizedPosition = 1 - normalizedScrollPosition;
        }

        private void RecalculateMaxEntries()
        {
            var viewportHeight = _scrollRect.viewport.rect.height;
            _visibleEntries = Mathf.CeilToInt(viewportHeight / EntryHeight);
            _maxEntries = _visibleEntries * 3;
        }

        private void UpdateArea()
        {
            var fullHeight = CandidateCollection.FilteredCount * EntryHeight;
            _contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fullHeight);
            _fullHeight = fullHeight;
        }

        public void UpdateState()
        {
            var cache = _cache;
            var existingEntries = _entries;

            var collection = CandidateCollection;
            var candidates = collection.FilteredEntries;

            var length = candidates.Length;
            var visibleEntries = _maxEntries;

            var viewportStartPosition = _normalizedScrollPosition * Mathf.Max(1, length - visibleEntries / 3f);
            var entriesStartPosition = viewportStartPosition - visibleEntries / 3f;

            var firstVisibleIndex = Mathf.Max(0, Mathf.FloorToInt(entriesStartPosition));
            var lastVisibleIndex = Mathf.Min(firstVisibleIndex + visibleEntries, length) - 1;

            for (var i = 0; i < visibleEntries; i++)
            {
                var index = firstVisibleIndex + i;
                if (index > lastVisibleIndex)
                {
                    break;
                }

                if (index >= candidates.Length)
                {
                    Debug.LogError("Index: " + index + ", Candidates: " + candidates.Length);
                }

                var existing = existingEntries.FirstOrDefault(e => e.Index == index);
                if (existing)
                {
                    continue;
                }

                var available = existingEntries.FirstOrDefault(e =>
                    e.Index < firstVisibleIndex ||
                    e.Index > lastVisibleIndex
                );
                if (!available)
                {
                    available = cache.Count > 0 ? cache.Dequeue() : CreateEntry();
                }

                var candidate = candidates[index];
                available.Rank = Ranks.TryGetValue(candidate, out var r) ? r + 1 : 0;
                available.Index = index;
                available.Candidate = candidate;
                available.Position = index * EntryHeight;
                available.gameObject.SetActive(true);
                existingEntries.Add(available);
            }

            for (var i = existingEntries.Count - 1; i >= 0; i--)
            {
                var entry = existingEntries[i];
                if (entry.Index >= firstVisibleIndex && entry.Index <= lastVisibleIndex)
                {
                    continue;
                }

                entry.Index = -1;

                existingEntries.RemoveAt(i);
                entry.gameObject.SetActive(false);
                cache.Enqueue(entry);
            }
        }

        private LeaderboardEntryController CreateEntry()
        {
            var instanceObject = Instantiate(_template.gameObject, _entriesArea, false);
            instanceObject.SetActive(true);
            var result = instanceObject.GetComponent<LeaderboardEntryController>();
            result.Initialize();
            result.Selected += () => OnEntrySelected(result);
            return result;
        }
    }
}