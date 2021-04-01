using System.Collections.Generic;
using Localizator.Settings;
using UnityEngine;

namespace Localizator
{
    public sealed class LocalizationScope : MonoBehaviour, ILocalizationScope
    {
        private readonly List<ILocalizable> _texts = new List<ILocalizable>();
        private ILocalization _localization = new DefaultLocalization();

        public ILocalization Localization
        {
            get => _localization;
            set => ApplyLocalization(value);
        }

        private void OnEnable()
        {
            UpdateLabels();
        }

        public void UpdateLabels()
        {
            foreach (var t in _texts)
            {
                t.UpdateLabels();
            }
        }

        public void Add(ILocalizable l)
        {
            _texts.Add(l);
        }

        public void Remove(ILocalizable t)
        {
            _texts.Remove(t);
        }

        private void ApplyLocalization(ILocalization localization)
        {
            _localization = localization;
            UpdateLabels();
        }
    }
}