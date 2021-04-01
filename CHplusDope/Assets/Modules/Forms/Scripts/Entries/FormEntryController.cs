using System;
using Localizator;
using Newtonsoft.Json.Linq;
using TMPro;
using Tooltips;
using UnityEngine;

namespace Forms
{
    public abstract class FormEntryController : MonoBehaviour
    {
        [SerializeField] private LocalizedText _nameText = null;
        [SerializeField] private LocalizedText _tooltipText = null;
        [SerializeField] private CanvasGroup _tooltipGroup = null;

        private FormController _form;
        private FormEntry _configuration = null;
        private bool _visible;

        public FormController Form => _form;
        protected ILocalization Localization => _form.Localization;

        public string ValueDependency { get; protected set; } = null;

        public bool Visible
        {
            get => _visible;
            set => SetVisible(value);
        }

        internal void Initialize(FormController form, FormEntry configuration)
        {
            _form = form;
            OnInitialize();
            ApplyEntry(configuration);
        }
        
        protected virtual void OnInitialize(){}

        protected void Start()
        {
            // Unity's Layout system is garbage. (This forces a layout rebuild)
            gameObject.SetActive(false);
            gameObject.SetActive(true);

            if (_tooltipGroup) _tooltipGroup.gameObject.SetActive(false);
        }

        public virtual bool Validate()
        {
            return true;
        }

        protected virtual void ApplyEntry(FormEntry entry)
        {
            _configuration = entry;
            if (_nameText)
            {
                _nameText.key = entry.Label;
                var display = !string.IsNullOrWhiteSpace(_nameText.text);
                _nameText.gameObject.SetActive(display);
            }
            if (_tooltipText)
            {
                _tooltipText.key = entry.Description;
            }
        }

        public virtual void SaveValues(JObject data)
        {
        }

        public virtual void ApplyDefaults()
        {
        }

        private void SetVisible(bool visible)
        {
            _visible = visible;
            gameObject.SetActive(visible);
        }

        internal void Remove()
        {
            Destroy(gameObject);
        }

        public virtual void OnDependencyUpdated()
        {
        }

        public void Interact(Touch touch)
        {
        }

        public void Focus(Touch touch)
        {
        }

        public void TouchDown(Touch touch)
        {
        }

        public void TouchUp(Touch touch)
        {
        }

        public void TouchEnter(Touch touch)
        {
            if (_tooltipGroup && !string.IsNullOrWhiteSpace(_tooltipText.text))
                _tooltipGroup.gameObject.SetActive(true);
        }

        public void TouchLeave(Touch touch)
        {
            if (_tooltipGroup) _tooltipGroup.gameObject.SetActive(false);
        }

        public virtual TooltipController ShowTooltip(Touch touch)
        {
            return null;
        }

        public void EvaluateOutlineState()
        {
        }
    }

    public abstract class FormEntryController<T> : FormEntryController where T : FormEntry
    {
        protected sealed override void ApplyEntry(FormEntry entry)
        {
            if (!(entry is T t))
            {
                throw new InvalidOperationException("Cannot apply configuration of type " +
                                                    entry.GetType().Name + " to entry of type " +
                                                    GetType().Name + "!");
            }
            base.ApplyEntry(entry);
            ApplyEntry(t);
        }

        protected abstract void ApplyEntry(T entry);
    }
}