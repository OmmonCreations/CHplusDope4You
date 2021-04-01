using System.Collections.Generic;
using System.Linq;
using Forms.Types;
using Localizator;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Forms
{
    public class FormController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private RectTransform _entriesArea = null;

        [SerializeField] private FormTemplates _templates = null;
        [SerializeField] private LocalizationScope _localizationScope = null;

        private Form _form;
        private FormEntryController[] _entries = null;
        private bool _changed = false;

        public ILocalization Localization => _localizationScope.Localization;
        private FormTemplates Templates => _templates;

        public Form Form
        {
            get => _form;
            set => ApplyForm(value);
        }

        private void LateUpdate()
        {
            if (_changed)
            {
                UpdateDependencies();
                _changed = false;
                if (_form.onValueChanged != null) _form.onValueChanged(GetFormData());
            }
        }

        private void ApplyForm(Form configuration)
        {
            _localizationScope.Localization = configuration.Localization != null
                ? configuration.Localization
                : new DefaultLocalization();
            _form = configuration;
            CreateEntries(configuration);
            _entriesArea.offsetMax = _entriesArea.offsetMax;
        }

        public void SetFormData(FormData data)
        {
            foreach (var entry in _entries.OfType<ValueEntryController>())
            {
                var target = entry.Key;
                var value = data.Get(target);
                entry.Value = value;
            }

            UpdateDependencies();
            _changed = false;
        }

        public FormData GetFormData()
        {
            var json = new JObject();
            foreach (var entry in _entries)
            {
                entry.SaveValues(json);
            }

            return new FormData(json);
        }

        public JToken GetValue(string key)
        {
            if (_entries == null) return null;
            return _entries.OfType<ValueEntryController>().Where(e => e.Key == key).Select(e => e.Value)
                .FirstOrDefault();
        }

        private void UpdateDependencies()
        {
            if (_entries == null) return;
            foreach (var entry in _entries.Where(e => e.ValueDependency != null))
            {
                entry.OnDependencyUpdated();
            }
        }

        private void ClearEntries()
        {
            if (_entries == null) return;
            foreach (var entry in _entries)
            {
                entry.Remove();
            }
        }

        private void CreateEntries(Form form)
        {
            ClearEntries();
            _entries = CreateEntries(_entriesArea, form.Entries);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        public bool Validate()
        {
            return _entries.All(e => e.Validate());
        }

        public static JObject Merge(JObject baseObject, JObject other)
        {
            var result = (JObject) baseObject.DeepClone();
            foreach (var entry in other)
            {
                result[entry.Key] = entry.Value.DeepClone();
            }

            return result;
        }

        internal FormEntryController[] CreateEntries(RectTransform parent, FormEntry[] configurations)
        {
            return configurations
                .Select(configuration => CreateEntry(parent, configuration))
                .Where(entry => entry).ToArray();
        }

        internal FormEntryController CreateEntry(RectTransform parent, FormEntry configuration)
        {
            FormEntryController result;
            switch (configuration)
            {
                case HeaderEntry _:
                    result = CreateEntry(Templates.headerEntryTemplate, parent);
                    break;
                case ArrayEntry _:
                    result = CreateEntry(Templates.arrayEntryTemplate, parent);
                    break;
                case BoolEntry _:
                    result = CreateEntry(Templates.boolEntryTemplate, parent);
                    break;
                case MultiSelectEntry _:
                    result = CreateEntry(Templates.multiSelectEntryTemplate, parent);
                    break;
                case FloatEntry _:
                    result = CreateEntry(Templates._floatEntryTemplate, parent);
                    break;
                case SliderEntry _:
                    result = CreateEntry(Templates.sliderEntryTemplate, parent);
                    break;
                case SectionEntry _:
                    result = CreateEntry(Templates.sectionEntryTemplate, parent);
                    break;
                case SelectEntry _:
                    result = CreateEntry(Templates.selectEntryTemplate, parent);
                    break;
                case StringEntry _:
                    result = CreateEntry(Templates.stringEntryTemplate, parent);
                    break;
                default:
                    Debug.LogError("Type " + configuration.GetType().Name + " not found.");
                    return null;
            }

            if (result == null)
            {
                Debug.LogError("No entry generated!");
                return null;
            }

            result.Initialize(this, configuration);
            return result;
        }

        internal T CreateEntry<T>(T prefab, RectTransform parent) where T : FormEntryController
        {
            var instanceObject = Instantiate(prefab.gameObject, parent, false);
            var instance = instanceObject.GetComponent<T>();
            return instance;
        }

        internal void TriggerChange()
        {
            _changed = true;
        }

        public void ApplyDefaults()
        {
            if (_entries == null) return;
            foreach (var entry in _entries)
            {
                entry.ApplyDefaults();
            }
        }
    }
}