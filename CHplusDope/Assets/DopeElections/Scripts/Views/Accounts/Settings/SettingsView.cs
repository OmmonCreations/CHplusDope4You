using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppSettings;
using CameraSystems;
using DopeElections.Localizations;
using Essentials;
using Localizator;
using Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Accounts
{
    public class SettingsView : AccountView
    {
        public override NamespacedKey Id => AccountViewId.Settings;

        [SerializeField] private TMP_Dropdown _languageDropdown = null;
        [SerializeField] private Slider _masterVolumeSlider = null;
        [SerializeField] private Slider _musicVolumeSlider = null;
        [SerializeField] private Slider _uiVolumeSlider = null;
        [SerializeField] private Slider _effectVolumeSlider = null;
        [SerializeField] private Slider _ambientVolumeSlider = null;
        [SerializeField] private Button _deleteDownloadedDataButton = null;
        [SerializeField] private Button _deleteAllDataButton = null;
        [SerializeField] private Button _backButton = null;
        [SerializeField] private PopupsLayer _popupsLayer = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private CameraTransformation _cameraTransformation = default;
        [SerializeField] private CameraSystem _cameraSystem = null;

        [SerializeField] private CanvasGroup _deleteDownloadsGroup = null;

        [SerializeField] private LocalizedText _titleText = null;
        [SerializeField] private LocalizedText _languagePlaceholderText = null;
        [SerializeField] private LocalizedText _masterVolumeText = null;
        [SerializeField] private LocalizedText _musicVolumeText = null;
        [SerializeField] private LocalizedText _uiVolumeText = null;
        [SerializeField] private LocalizedText _effectVolumeText = null;
        [SerializeField] private LocalizedText _ambientVolumeText = null;
        [SerializeField] private LocalizedText _deleteDownloadedDataText = null;
        [SerializeField] private LocalizedText _deleteAllDataText = null;
        [SerializeField] private LocalizedText _instructionText = null;
        [SerializeField] private LocalizedText _confirmText = null;

        private SettingsContainer Settings { get; set; }
        private Language[] Languages { get; set; }

        private string _previousLanguage;
        private bool _deletedDownloadedData = false;

        private readonly Dictionary<SettingType<float>, float> _previousVolumes =
            new Dictionary<SettingType<float>, float>();
        
        private bool DeletedDownloadedData
        {
            get => _deletedDownloadedData;
            set
            {
                _deletedDownloadedData = value;
                _deleteDownloadsGroup.alpha = value ? 0.2f : 1;
                _deleteDownloadsGroup.blocksRaycasts = !value;
                _deleteDownloadsGroup.interactable = !value;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Settings = DopeElectionsApp.Instance.Settings;

            _languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
            _masterVolumeSlider.onValueChanged.AddListener(v => OnVolumeChanged(Setting.MasterVolume, v));
            _musicVolumeSlider.onValueChanged.AddListener(v => OnVolumeChanged(Setting.MusicVolume, v));
            _uiVolumeSlider.onValueChanged.AddListener(v => OnVolumeChanged(Setting.UIVolume, v));
            _effectVolumeSlider.onValueChanged.AddListener(v => OnVolumeChanged(Setting.SfxVolume, v));
            _ambientVolumeSlider.onValueChanged.AddListener(v => OnVolumeChanged(Setting.AmbienceVolume, v));
            _deleteDownloadedDataButton.onClick.AddListener(RequestDeleteDownloadedData);
            _deleteAllDataButton.onClick.AddListener(RequestDeleteAllData);
            _backButton.onClick.AddListener(SaveAndQuit);
            // _confirmButton.onClick.AddListener(SaveAndQuit);

            _titleText.key = LKey.Views.Settings.Title;
            _languagePlaceholderText.key = LKey.Views.Settings.Language;
            _masterVolumeText.key = LKey.Views.Settings.MasterVolume;
            _musicVolumeText.key = LKey.Views.Settings.MusicVolume;
            _uiVolumeText.key = LKey.Views.Settings.UiVolume;
            _effectVolumeText.key = LKey.Views.Settings.EffectVolume;
            _ambientVolumeText.key = LKey.Views.Settings.AmbienceVolume;
            _deleteDownloadedDataText.key = LKey.Views.Settings.DeleteDownloadedData;
            _deleteAllDataText.key = LKey.Views.Settings.DeleteAllData;
            _instructionText.key = LKey.Views.Settings.Instruction;
            _confirmText.key = LKey.Views.Settings.Confirm;

            UpdateLanguageOptions();
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            var language = Languages.FirstOrDefault(l => l.code == Settings.GetValue(Setting.Language));
            var masterVolume = Settings.GetValue(Setting.MasterVolume);
            var musicVolume = Settings.GetValue(Setting.MusicVolume);
            var uiVolume = Settings.GetValue(Setting.UIVolume);
            var sfxVolume = Settings.GetValue(Setting.SfxVolume);
            var ambienceVolume = Settings.GetValue(Setting.AmbienceVolume);

            _languageDropdown.SetValueWithoutNotify(Languages.IndexOf(language));
            _masterVolumeSlider.SetValueWithoutNotify(masterVolume);
            _musicVolumeSlider.SetValueWithoutNotify(musicVolume);
            _uiVolumeSlider.SetValueWithoutNotify(uiVolume);
            _effectVolumeSlider.SetValueWithoutNotify(sfxVolume);
            _ambientVolumeSlider.SetValueWithoutNotify(ambienceVolume);

            _previousLanguage = language.code;
            _previousVolumes[Setting.MasterVolume] = masterVolume;
            _previousVolumes[Setting.MusicVolume] = musicVolume;
            _previousVolumes[Setting.UIVolume] = uiVolume;
            _previousVolumes[Setting.SfxVolume] = sfxVolume;
            _previousVolumes[Setting.AmbienceVolume] = ambienceVolume;
            
            DeletedDownloadedData = false;

            _cameraSystem.CurrentTransform = _cameraTransformation;
            Views.BlackMask.BlockInteractions(false);
            if (Views.BlackMask.Alpha > 0) Views.BlackMask.FadeToClear();

            StartCoroutine(SetScrollPosition());
        }

        private void SaveAndQuit()
        {
            var app = DopeElectionsApp.Instance;
            app.SaveSettings();
            if (app.Settings.GetValue(Setting.Language) != _previousLanguage || DeletedDownloadedData)
            {
                Views.BlackMask.BlockInteractions(true);
                Views.BlackMask.FadeToBlack(DopeElectionsApp.Instance.ReloadAssets);
                return;
            }

            Quit();
        }

        private void QuitWithoutSaving()
        {
            Reset();
            Quit();
        }

        private void Quit()
        {
            Views.BlackMask.FadeToBlack(() => { DopeElectionsRouter.GoToMainMenu(); });
        }

        private void Reset()
        {
            Settings.SetValue(Setting.Language, _previousLanguage);
            Settings.SetValue(Setting.MasterVolume, _previousVolumes[Setting.MasterVolume]);
            Settings.SetValue(Setting.MusicVolume, _previousVolumes[Setting.MusicVolume]);
            Settings.SetValue(Setting.UIVolume, _previousVolumes[Setting.UIVolume]);
            Settings.SetValue(Setting.SfxVolume, _previousVolumes[Setting.SfxVolume]);
            Settings.SetValue(Setting.AmbienceVolume, _previousVolumes[Setting.AmbienceVolume]);
        }

        private void OnLanguageSelected(int value)
        {
            var language = value >= 0 && value < Languages.Length ? Languages[value] : Languages[0];
            Settings.SetValue(Setting.Language, language.code);
        }

        private void OnVolumeChanged(SettingType<float> setting, float value)
        {
            Settings.SetValue(setting, value);
        }

        private void UpdateLanguageOptions()
        {
            Languages = Setting.Language.AvailableLanguages;
            _languageDropdown.options = Languages.Select(l => new TMP_Dropdown.OptionData(l.name)).ToList();
        }

        private void RequestDeleteDownloadedData()
        {
            _popupsLayer.ShowPopup(
                new ConfirmPopup(
                    LKey.Views.Settings.DeleteDownloadedDataPrompt.Title,
                    LKey.Views.Settings.DeleteDownloadedDataPrompt.Description,
                    LKey.Views.Settings.DeleteDownloadedDataPrompt.Confirm,
                    LKey.Views.Settings.DeleteDownloadedDataPrompt.Cancel
                ).Then(DeleteDownloadedData)
            );
        }

        private void RequestDeleteAllData()
        {
            _popupsLayer.ShowPopup(
                new ConfirmPopup(
                    LKey.Views.Settings.DeleteAllDataPrompt.Title,
                    LKey.Views.Settings.DeleteAllDataPrompt.Description,
                    LKey.Views.Settings.DeleteAllDataPrompt.Confirm,
                    LKey.Views.Settings.DeleteAllDataPrompt.Cancel
                ).Then(DeleteAllData)
            );
        }

        private void DeleteDownloadedData()
        {
            DopeElectionsApp.Instance.DeleteDownloadedData();
            DeletedDownloadedData = true;
        }

        private void DeleteAllData()
        {
            DopeElectionsApp.Instance.DeleteAllData();
        }

        private IEnumerator SetScrollPosition()
        {
            yield return null;
            _scrollRect.normalizedPosition = Vector2.up;
        }
    }
}