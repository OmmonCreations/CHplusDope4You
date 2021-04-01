using System.Collections.Generic;
using Localizator.Settings;

namespace Localizator
{
    public sealed class GlobalLocalizationScope : ILocalizationScope
    {
        public static readonly GlobalLocalizationScope current = new GlobalLocalizationScope();
        
        private readonly List<ILocalizable> _texts = new List<ILocalizable>();
        private ILocalization _localization = new DefaultLocalization();

        private Language _language;
        
        public ILocalization Localization
        {
            get => _localization;
            set
            {
                _localization = value;
                UpdateLabels();
            }
        }
        
        public void UpdateLabels()
        {
            foreach (var t in _texts)
            {
                t.UpdateLabels();
            }
        }

        public void Add(ILocalizable t)
        {
            _texts.Add(t);
        }

        public void Remove(ILocalizable t)
        {
            _texts.Remove(t);
        }
    }
}