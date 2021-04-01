using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DopeElections.Races
{
    public class RaceProgressDisplayController : MonoBehaviour
    {
        [SerializeField] private RectTransform _viewport = null;
        [SerializeField] private RectTransform _entriesArea = null;
        [SerializeField] private float _minEntryWidth = 80;
        [SerializeField] private float _maxEntryWidth = 120;
        [SerializeField] private float _viewportExtraMargin = 25;
        [SerializeField] private RaceProgressDisplayEntryController _entryTemplate = null;
        [SerializeField] private RaceProgressDisplayEntryController _lastEntry = null;

        private RaceProgressDisplayEntryController[] _entries = null;
        private Action<int> _entrySelectAction;

        public void Initialize(IReadOnlyList<bool> steps, Action<int> entrySelectAction)
        {
            _entryTemplate.gameObject.SetActive(false);
            ClearEntries();
            _entries = CreateEntries(steps);
            _lastEntry.SetCompletedImmediate(steps.All(s => s));
            _entrySelectAction = entrySelectAction;
        }

        private void ClearEntries()
        {
            if (_entries == null) return;
            foreach (var e in _entries) e.Remove();
            _entries = null;
        }

        private RaceProgressDisplayEntryController[] CreateEntries(IReadOnlyList<bool> steps)
        {
            var viewport = _viewport;
            var count = steps.Count;
            var viewportWidth = viewport.rect.width - _viewportExtraMargin;
            var entryWidth = Mathf.Clamp((viewportWidth - _lastEntry.Width) / count, _minEntryWidth, _maxEntryWidth);
            var result = new RaceProgressDisplayEntryController[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = CreateEntry(i, steps[i], entryWidth);
            }

            return result;
        }

        private RaceProgressDisplayEntryController CreateEntry(int index, bool cleared, float width)
        {
            var instanceObject = Instantiate(_entryTemplate.gameObject, _entriesArea, false);
            var instance = instanceObject.GetComponent<RaceProgressDisplayEntryController>();
            instance.SetCompletedImmediate(cleared);
            instance.Width = width;
            instance.onClick.AddListener(() => OnEntrySelected(index));
            instanceObject.SetActive(true);
            return instance;
        }

        private void OnEntrySelected(int index)
        {
            if (_entrySelectAction != null) _entrySelectAction(index);
        }

        public void SetActive(int step, bool active = true)
        {
            var entry = step >= 0 && step < _entries.Length ? _entries[step] : null;
            if (entry == null) return;
            if(active) foreach (var e in _entries.Where(e => e != entry && e.IsActive)) e.SetActive(false);
            entry.SetActive(active);
        }

        public void SetCompleted(int step, bool completed = true)
        {
            var entry = step >= 0 && step < _entries.Length ? _entries[step] : null;
            if (entry == null) return;
            if (step == _entries.Length - 1 && completed)
            {
                entry.SetCompleted(true, () => _lastEntry.SetCompleted(true));
            }
            else if (step == _entries.Length - 1 && !completed)
            {
                _lastEntry.SetCompleted(false, () => entry.SetCompleted(false));
            }
            else entry.SetCompleted(completed);
        }

        public void JumpTo(int active)
        {
            var entries = _entries;
            if (entries == null) return;

            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                entry.SetCompletedImmediate(i < active);
                entry.SetActiveImmediate(i == active);
            }
        }
    }
}