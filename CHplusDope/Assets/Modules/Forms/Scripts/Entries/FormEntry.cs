using Localizator;

namespace Forms
{
    public abstract class FormEntry
    {
        public LocalizationKey Label { get; set; }
        public LocalizationKey Description { get; set; }
    }
}