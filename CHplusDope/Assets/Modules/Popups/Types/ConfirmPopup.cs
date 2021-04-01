using System;
using Localizator;

namespace Popups
{
    public class ConfirmPopup : Popup, ICancellablePopup<ConfirmPopup>
    {
        public delegate void ActionEvent();

        public event ActionEvent Confirmed = delegate { };
        public event ActionEvent Cancelled = delegate { };

        public LocalizationKey ConfirmText { get; }
        public LocalizationKey CancelText { get; }

        public ConfirmPopup(LocalizationKey title, LocalizationKey text) : base(title, text)
        {
            ConfirmText = new LocalizationKey {fallback = "Confirm"};
            CancelText = new LocalizationKey {fallback = "Cancel"};
        }

        public ConfirmPopup(LocalizationKey title, LocalizationKey text, LocalizationKey confirm, LocalizationKey cancel) : base(title, text)
        {
            ConfirmText = confirm;
            CancelText = cancel;
        }

        public ConfirmPopup Then(Action callback)
        {
            Confirmed += callback.Invoke;
            return this;
        }

        public ConfirmPopup Else(Action callback)
        {
            Cancelled += callback.Invoke;
            return this;
        }

        public void Confirm()
        {
            if (IsClosed) return;
            Close();
            Confirmed();
        }

        public void Cancel()
        {
            if (IsClosed) return;
            Close();
            Cancelled();
        }
    }
}