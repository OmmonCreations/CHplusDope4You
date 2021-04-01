using System.Collections.Generic;
using System.Linq;
using StateMachines;
using UnityEngine;

namespace Progression.UI
{
    public abstract class ProgressionTreeController : MonoBehaviour
    {
        private ProgressionTree _tree;
        private ProgressEntryController[] _controllers;
        private bool _interactable;

        public ProgressEntryController[] Controllers => _controllers;

        public bool ShowsLabels { get; private set; }

        public bool Interactable
        {
            get => _interactable;
            set => ApplyInteractable(value);
        }

        public ProgressionTree Tree
        {
            get => _tree;
            set => ApplyTree(value);
        }

        private void Awake()
        {
            // _template.gameObject.SetActive(false);
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
        }

        public void Focus(IVisibleProgressEntry entry)
        {
            Focus(Controllers.FirstOrDefault(c => c.Entry == entry));
        }

        public void FocusImmediate(IVisibleProgressEntry entry)
        {
            FocusImmediate(Controllers.FirstOrDefault(c => c.Entry == entry));
        }

        public void ShowLabels() => ShowLabels(true);
        public void HideLabels() => ShowLabels(false);

        public void ShowLabels(bool show)
        {
            ShowsLabels = show;
            UpdateVisibleLabels();
            OnShowLabelsChanged(show);
        }

        public void ShowLabelsImmediate() => ShowLabelsImmediate(true);
        public void HideLabelsImmediate() => ShowLabelsImmediate(false);

        public void ShowLabelsImmediate(bool show)
        {
            ShowsLabels = show;
            UpdateVisibleLabelsImmediate();
            OnShowLabelsChanged(show);
        }
        
        protected void UpdateVisibleLabels()
        {
            var controllers = Controllers;
            foreach (var c in controllers)
            {
                var labelVisible = ShowsLabels && IsLabelVisible(c);
                if (c.IsLabelVisible == labelVisible) continue;
                c.ShowLabel(labelVisible);
            }
        }

        protected void UpdateVisibleLabelsImmediate()
        {
            var controllers = Controllers;
            foreach (var c in controllers)
            {
                var labelVisible = IsLabelVisible(c);
                if (c.IsLabelVisible == labelVisible) continue;
                c.ShowLabelImmediate(labelVisible);
            }
        }

        private void ApplyTree(ProgressionTree tree)
        {
            _tree = tree;
            ClearEntries();
            var controllers = CreateEntries(tree);
            foreach (var c in controllers)
            {
                c.onClick.AddListener(() => Select(c));
            }

            _controllers = controllers;

            OnTreeChanged();
        }

        private void ApplyInteractable(bool interactable)
        {
            _interactable = interactable;
            if (Controllers != null)
            {
                foreach (var c in Controllers) c.Interactable = interactable;
            }
            
            OnInteractableChanged(interactable);
        }

        protected virtual void OnTreeChanged()
        {
        }

        protected virtual void OnInteractableChanged(bool interactable)
        {
            
        }

        private void ClearEntries()
        {
            if (_controllers == null) return;
            OnBeforeClearEntries();
            foreach (var e in _controllers)
            {
                e.Remove();
            }

            _controllers = null;
        }

        protected abstract void Select(ProgressEntryController entry);
        protected abstract void Focus(ProgressEntryController entry);
        protected abstract void FocusImmediate(ProgressEntryController entry);

        protected abstract bool IsLabelVisible(ProgressEntryController entry);

        protected virtual void OnBeforeClearEntries()
        {
            
        }

        protected virtual void OnShowLabelsChanged(bool show)
        {
        }

        protected abstract ProgressEntryController[] CreateEntries(ProgressionTree tree);
    }

    public abstract class ProgressionTreeController<T> : ProgressionTreeController where T : ProgressEntryController
    {
        public delegate void UserEvent(T entry);

        public event UserEvent EntrySelected = delegate { };

        public new T[] Controllers => base.Controllers.OfType<T>().ToArray();

        private T _focused;
        
        public T Focused => _focused;
        
        public virtual void Select(T entry)
        {
            Focus(entry);
            TriggerEntrySelected(entry);
        }

        protected void TriggerEntrySelected(T entry)
        {
            EntrySelected(entry);
        }

        public void Focus(T entry)
        {
            _focused = entry;
            OnFocus(entry);
        }

        public void FocusImmediate(T entry)
        {
            _focused = entry;
            OnFocusImmediate(entry);
        }

        protected virtual void OnFocus(T entry)
        {
        }

        protected virtual void OnFocusImmediate(T entry)
        {
        }

        protected virtual bool IsLabelVisible(T entry)
        {
            return entry == Focused && ShowsLabels;
        }

        protected override void Focus(ProgressEntryController entry)
        {
            Focus(entry as T);
        }

        protected override void FocusImmediate(ProgressEntryController entry)
        {
            FocusImmediate(entry as T);
        }

        protected sealed override void Select(ProgressEntryController entry)
        {
            Select(entry as T);
        }

        protected sealed override bool IsLabelVisible(ProgressEntryController entry)
        {
            return IsLabelVisible(entry as T);
        }
    }
}