using Localizator;

namespace Popups
{
    public abstract class Popup
    {
        public delegate void CloseEvent();

        public event CloseEvent Closed = delegate { };
        
        public LocalizationKey Title { get; }
        public LocalizationKey Text { get; }

        private bool _closed = false;

        public bool IsClosed => _closed;

        protected Popup(LocalizationKey title, LocalizationKey text)
        {
            Title = title;
            Text = text;
        }

        public void Close()
        {
            if (_closed) return;
            _closed = true;
            Closed();
        }
    }
}