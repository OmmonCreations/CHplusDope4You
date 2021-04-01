using System;
using Localizator;

namespace Popups
{
    public class AlertPopup : Popup, IPopup<AlertPopup>
    {
        public delegate void ConfirmEvent();

        public event ConfirmEvent Confirmed = delegate { };

        public LocalizationKey ConfirmText { get; }

        public AlertPopup(LocalizationKey title, LocalizationKey text) : base(title, text)
        {
            ConfirmText = new LocalizationKey {fallback = "Ok"};
        }

        public AlertPopup(LocalizationKey title, LocalizationKey text, LocalizationKey confirm) : base(title, text)
        {
            ConfirmText = confirm;
        }

        public AlertPopup Then(Action callback)
        {
            Confirmed += callback.Invoke;
            return this;
        }

        public void Confirm()
        {
            if (IsClosed) return;
            Close();
            Confirmed();
        }
    }
}