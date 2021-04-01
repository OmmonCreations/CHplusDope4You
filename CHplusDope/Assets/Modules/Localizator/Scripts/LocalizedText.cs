using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Localizator
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LocalizedText : MonoBehaviour, ILocalizable
    {
        [SerializeField] private UnityEvent _onUpdate = new UnityEvent();
        
        private LocalizationKey _localizationKey;
        private readonly Dictionary<string, string> _variables = new Dictionary<string, string>();

        private TMP_Text _text = null;
        private ILocalizationScope _scope;

        public TMP_Text textComponent => _text;
        public UnityEvent onUpdate => _onUpdate;

        public ILocalizationScope Scope
        {
            get
            {
                if (_scope != null) return _scope;
                _scope = GetComponentInParent<ILocalizationScope>() ?? GlobalLocalizationScope.current;
                return _scope;
            }
        }

        public string text
        {
            get
            {
                if (!_text) _text = GetComponent<TMP_Text>();
                return _text.text;
            }
            set
            {
                Debug.LogWarning("Assigned localized text directly (" + value + ")! This is not recommended.");
                _text.text = value;
            }
        }

        public LocalizationKey key
        {
            get
            {
                return _localizationKey;
            }
            set
            {
                _localizationKey = value;
                if (enabled && gameObject.activeInHierarchy) UpdateLabels();
            }
        }

        public ILocalization localization => Scope.Localization;

        private void Start()
        {
#if UNITY_EDITOR
            if (gameObject.GetComponent<LocalizedTextEditor>() || !Application.isPlaying) return;
            var editor = gameObject.AddComponent<LocalizedTextEditor>();
            editor.target = this;
#endif
        }

        private void OnEnable()
        {
            if (!_text) _text = GetComponent<TMP_Text>();
            _scope = GetComponentInParent<ILocalizationScope>();
            Scope.Add(this);
            UpdateLabels();
        }

        private void OnDisable()
        {
            if (_scope != null) _scope.Remove(this);
        }

        private void OnDestroy()
        {
            if (_scope != null) _scope.Remove(this);
        }

        public void SetVariable(string key, string value)
        {
            if (value != null) _variables[key] = value;
            else _variables.Remove(key);
            if (enabled) UpdateLabels();
        }

        public void UpdateLabels()
        {
            if (!_text) return;
            var key = this.key;
            string value = null;
            var useFallback = _scope == null || _scope.Localization == null ||
                              !_scope.Localization.TryGetString(key, out value);

            var text = !useFallback ? value : key.fallback;
            if (_variables != null)
            {
                text = LocalizationUtility.ApplyReplacements(text, _variables);
            }

            _text.text = text;
            onUpdate.Invoke();
        }
    }
}