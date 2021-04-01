using Localizator;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Popups
{
    public class AlertPopupController : PopupController<AlertPopup>
    {
        [SerializeField] private LocalizedText _title = null;
        [SerializeField] private LocalizedText _text = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private Button _confirmButton = null;

        public UnityEvent OnConfirm => _confirmButton.onClick;
        
        protected override void OnInitialize()
        {
            var popup = Popup;
            if(_title) _title.key = popup.Title;
            _text.key = popup.Text;
            _confirmText.key = popup.ConfirmText;

            _confirmButton.onClick.AddListener(popup.Confirm);
        }
    }
}