using Localizator;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Popups
{
    public class ConfirmPopupController : PopupController<ConfirmPopup>
    {
        [SerializeField] private LocalizedText _title = null;
        [SerializeField] private LocalizedText _text = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private LocalizedText _cancelText = null;
        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private Button _cancelButton = null;

        public UnityEvent OnConfirm => _confirmButton.onClick;
        public UnityEvent OnCancel => _cancelButton.onClick;

        protected override void OnInitialize()
        {
            var popup = Popup;
            if(_title) _title.key = popup.Title;
            _text.key = popup.Text;
            _confirmText.key = popup.ConfirmText;
            _cancelText.key = popup.CancelText;

            _confirmButton.onClick.AddListener(popup.Confirm);
            _cancelButton.onClick.AddListener(popup.Cancel);
        }
    }
}