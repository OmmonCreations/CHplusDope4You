using System;
using Localizator;

namespace Popups
{
    public class PromptPopup : Popup, ICancellablePopup<PromptPopup,string>
    {
        public delegate void ActionEvent(string value);

        public event ActionEvent Confirmed = delegate { };
        public event ActionEvent Cancelled = delegate { };

        public string DefaultValue { get; }
        public LocalizationKey PlaceholderText { get; }
        public LocalizationKey ConfirmText { get; }
        public LocalizationKey CancelText { get; }

        public PromptPopup(LocalizationKey title, LocalizationKey text, string value = null) : base(title, text)
        {
            DefaultValue = value;
            PlaceholderText = new LocalizationKey {fallback = ""};
            ConfirmText = new LocalizationKey {fallback = "Confirm"};
            CancelText = new LocalizationKey {fallback = "Cancel"};
        }

        public PromptPopup(LocalizationKey title, LocalizationKey text, string value, LocalizationKey placeholder, LocalizationKey confirm,
            LocalizationKey cancel) : base(title, text)
        {
            DefaultValue = value;
            PlaceholderText = placeholder;
            ConfirmText = confirm;
            CancelText = cancel;
        }

        public PromptPopup Then(Action<string> callback)
        {
            Confirmed += callback.Invoke;
            return this;
        }

        public PromptPopup Else(Action<string> callback)
        {
            Cancelled += callback.Invoke;
            return this;
        }

        public void Confirm(string value)
        {
            if (IsClosed) return;
            Close();
            Confirmed(value);
        }

        public void Cancel(string value = null)
        {
            if (IsClosed) return;
            Close();
            Cancelled(value);
        }
    }
}