using System;
using System.Collections.Generic;
using System.Linq;
using AnimatedObjects;
using DopeElections.Answer;
using DopeElections.Candidates;
using DopeElections.ElectionLists;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DopeElections.SmartSpiders
{
    public class SmartSpiderController : UIBehaviour
    {
        public delegate void Event();

        public event Event CandidateChanged = delegate { };
        public event Event PreviousUserSpiderChanged = delegate { };
        public event Event ListChanged = delegate { };
        public event Event CompareListChanged = delegate { };

        private static readonly string[] Axis =
            {"axis1", "axis2", "axis3", "axis4", "axis5", "axis6", "axis7", "axis8"};

        [SerializeField] private Layer[] _layers = null;
        [SerializeField] private Button _infoButton = null;
        [SerializeField] private Button _statsButton = null;
        [SerializeField] private SmartSpiderControls _controls = null;
        [SerializeField] private ToggleableObjectController _smartSpiderAnimationController = null;

        private Candidate _candidate;
        private SmartSpider _previousUserSpider;
        private ElectionList _list;
        private ElectionList _compareList;

        public UnityEvent onInfoButtonClick => _infoButton.onClick;
        public UnityEvent onStatsButtonClick => _statsButton.onClick;

        public Candidate Candidate => _candidate;
        public SmartSpider PreviousUserSpider => _previousUserSpider;
        public ElectionList List => _list;
        public ElectionList CompareList => _compareList;
        public SmartSpiderControls Controls => _controls;

        private readonly Dictionary<Slot, SmartSpider> _display = new Dictionary<Slot, SmartSpider>();

        private bool initialized;

        public void Initialize()
        {
            if (initialized)
                return;

            if (Controls) Controls.Initialize(this);

            Set(Slot.User, new SmartSpider());
            Set(Slot.Candidate, new SmartSpider());
            Set(Slot.List1, new SmartSpider());
            Set(Slot.List2, new SmartSpider());
            Set(Slot.PreviousUser, new SmartSpider());

            initialized = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            foreach (var l in _layers) l.enabled = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            foreach (var l in _layers) l.enabled = false;
        }

        public void Show() => Show(true);
        public void Hide() => Show(false);
        public void Show(bool show) => _smartSpiderAnimationController.Show(show);

        public void ShowImmediate() => ShowImmediate(true);
        public void HideImmediate() => ShowImmediate(false);
        public void ShowImmediate(bool show) => _smartSpiderAnimationController.ShowImmediate(show);

        public void ShowUser(bool show = true)
        {
            if (show) Set(Slot.User, DopeElectionsApp.Instance.User.SmartSpider);
            else ClearValues(Slot.User);
        }

        public void ShowPreviousUser(SmartSpider smartSpider, bool show = true)
        {
            show &= smartSpider != null;
            if (show) Set(Slot.User, _previousUserSpider);
            else ClearValues(Slot.User);

            if (smartSpider != _previousUserSpider)
            {
                _previousUserSpider = smartSpider;
                PreviousUserSpiderChanged();
            }
        }

        public void ShowCandidate(Candidate candidate, bool show = true)
        {
            show &= candidate != null;
            if (show) Set(Slot.Candidate, candidate.smartSpider);
            else ClearValues(Slot.Candidate);

            if (candidate != _candidate)
            {
                _candidate = candidate;
                var layer = _layers.FirstOrDefault(l => l.slot == Slot.Candidate);
                if (layer != null) layer.smartSpider.color = candidate.GetPartyColor();
                CandidateChanged();
            }
        }

        public void ShowList(ElectionList list, bool show = true)
        {
            show &= list != null;
            if (show) Set(Slot.List1, list.GetSmartSpider());
            else ClearValues(Slot.List1);

            if (list != _list)
            {
                _list = list;
                var layer = _layers.FirstOrDefault(l => l.slot == Slot.List1);
                if (layer != null) layer.smartSpider.color = list.GetColor();
                ListChanged();
            }
        }

        public void ShowCompareList(ElectionList list, bool show = true)
        {
            show &= list != null;
            if (show) Set(Slot.List2, list.GetSmartSpider());
            else ClearValues(Slot.List2);

            if (list != _compareList)
            {
                _compareList = list;
                var layer = _layers.FirstOrDefault(l => l.slot == Slot.List1);
                if (layer != null) layer.smartSpider.color = list.GetColor();
                CompareListChanged();
            }
        }

        private void Set(Slot slot, SmartSpider spider)
        {
            _display[slot] = spider;
            ApplyValues(slot, spider);
        }

        private SmartSpider Get(Slot slot)
        {
            return _display.TryGetValue(slot, out var result) ? result : null;
        }

        public void Clear(Slot slot)
        {
            _display.Remove(slot);
            ClearValues(slot);
        }

        private void ApplyValues(Slot slot, SmartSpider smartSpider)
        {
            var layer = _layers.FirstOrDefault(l => l.slot == slot);
            if (layer == null)
            {
                Debug.LogError("Layer " + slot + " not found!");
                return;
            }

            layer.smartSpider.Value = smartSpider.Values;
        }

        public void ClearAllValue()
        {
            foreach (var l in _layers)
            {
                l.smartSpider.Value = new float[8];
            }
        }

        private void ClearValues(Slot slot)
        {
            var layer = _layers.FirstOrDefault(l => l.slot == slot);
            if (layer == null)
            {
                Debug.LogError("Layer " + slot + " not found!");
                return;
            }

            layer.smartSpider.Value = new float[8];
        }

        [Serializable]
        public class Layer
        {
            public Slot slot;
            public MiniSmartSpider smartSpider;

            public bool enabled
            {
                get => smartSpider.enabled;
                set => smartSpider.enabled = value;
            }
        }

        public enum Slot
        {
            User,
            Candidate,
            List1,
            List2,
            PreviousUser
        }
    }
}