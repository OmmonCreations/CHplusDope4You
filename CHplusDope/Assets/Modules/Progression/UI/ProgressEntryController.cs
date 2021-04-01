using Localizator;
using StateMachines;
using UnityEngine;
using UnityEngine.Events;

namespace Progression.UI
{
    public abstract class ProgressEntryController : MonoBehaviour
    {
        public string Id => "progress_entry_" + Entry.Id;

        [SerializeField] private UnityEvent _onClick = null;

        private LocalizationKey _label;
        private string _state;
        private bool _labelVisible;
        private bool _interactable;

        public ProgressionTreeController TreeController { get; private set; }
        public ProgressionTree Tree { get; private set; }
        public IVisibleProgressEntry Entry { get; private set; }
        public int Index { get; private set; }
        public bool CanShowLabel { get; set; }

        public string State
        {
            get => _state;
            set => ApplyState(value);
        }

        public bool Interactable
        {
            get => _interactable;
            set => ApplyInteractable(value);
        }

        public LocalizationKey Label
        {
            get => _label;
            set => ApplyLabel(value);
        }


        public UnityEvent onClick => _onClick;
        public bool IsLabelVisible => _labelVisible;

        public void Initialize(ProgressionTreeController treeController, IVisibleProgressEntry entry, int index,
            bool canShowLabel)
        {
            TreeController = treeController;
            Tree = treeController.Tree;
            Entry = entry;
            Index = index;
            State = entry.State;
            Label = entry.Label;
            CanShowLabel = canShowLabel;
            HideLabel();
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        private void ApplyState(string state)
        {
            _state = state;
            OnStateChanged(state);
        }

        private void ApplyInteractable(bool interactable)
        {
            _interactable = interactable;
            OnInteractableChanged(interactable);
        }

        protected virtual void OnStateChanged(string state)
        {
        }

        protected virtual void OnInteractableChanged(bool interactable)
        {
        }

        private void ApplyLabel(LocalizationKey label)
        {
            _label = label;
            OnLabelChanged(label);
        }

        protected virtual void OnLabelChanged(LocalizationKey label)
        {
        }

        public TransitionState ShowLabel(float delay = 0) => ShowLabel(true, delay);
        public TransitionState HideLabel(float delay = 0) => ShowLabel(false, delay);

        public TransitionState ShowLabel(bool show, float delay = 0)
        {
            if (show && !CanShowLabel) return null;
            _labelVisible = show;
            return CreateLabelTransitionState(show, delay);
        }

        public void ShowLabelImmediate(float delay = 0) => ShowLabelImmediate(true);
        public void HideLabelImmediate(float delay = 0) => ShowLabelImmediate(false);

        public void ShowLabelImmediate(bool show)
        {
            if (show && !CanShowLabel) return;
            _labelVisible = show;
            OnShowLabelImmediate(show);
        }

        protected virtual void OnShowLabelImmediate(bool show)
        {
        }

        protected abstract TransitionState CreateLabelTransitionState(bool show, float delay);
    }
}