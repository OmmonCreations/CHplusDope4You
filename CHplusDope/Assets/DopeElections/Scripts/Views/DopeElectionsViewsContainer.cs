using Localizator;
using UnityEngine.InputSystem;
using Views;

namespace DopeElections
{
    public abstract class DopeElectionsViewsContainer : ViewsContainer
    {
        private bool _hooksActive = false;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            HookEvents();
            UpdateLocalization();
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
            HookEvents();
            UpdateLocalization();
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            ReleaseHooks();
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                DopeElectionsApp.Instance.ReloadLocalization(localization => { Localization = localization; });
            }
        }

#endif

        private void HookEvents()
        {
            if (_hooksActive) return;
            if (DopeElectionsApp.Instance)
            {
                _hooksActive = true;
                DopeElectionsApp.Instance.LocalizationChanged += OnLocalizationChanged;
            }
        }

        private void ReleaseHooks()
        {
            _hooksActive = false;
            if (DopeElectionsApp.Instance) DopeElectionsApp.Instance.LocalizationChanged -= OnLocalizationChanged;
        }

        protected sealed override ILocalization FetchLocalization()
        {
            return DopeElectionsApp.Instance != null
                ? DopeElectionsApp.Instance.Localization
                : new DefaultLocalization();
        }

        protected void UpdateLocalization()
        {
            Localization = FetchLocalization();
        }

        private void OnLocalizationChanged(ILocalization localization)
        {
            Localization = localization;
        }
    }
}