using Localizator;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Popups
{
    public class PromptPopupController : PopupController<PromptPopup>
    {
        [SerializeField] private TMP_InputField _inputField = null;
        [SerializeField] private LocalizedText _title = null;
        [SerializeField] private LocalizedText _text = null;
        [SerializeField] private LocalizedText _placeholderText = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private LocalizedText _cancelText = null;
        [SerializeField] private Button _closeBackground = null;
        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private Button _cancelButton = null;

        public UnityEvent OnConfirm => _confirmButton.onClick;
        public UnityEvent OnCancel => _cancelButton.onClick;

        protected override void OnInitialize()
        {
            var popup = Popup;
            if(_title) _title.key = popup.Title;
            _text.key = popup.Text;
            _placeholderText.key = popup.PlaceholderText;
            _confirmText.key = popup.ConfirmText;
            _cancelText.key = popup.CancelText;

            _inputField.text = popup.DefaultValue;

            _inputField.onSubmit.AddListener(popup.Confirm);
            _confirmButton.onClick.AddListener(() => popup.Confirm(_inputField.text));
            _cancelButton.onClick.AddListener(() => popup.Cancel());
            _closeBackground.onClick.AddListener(() => popup.Cancel());
        }
    }
}