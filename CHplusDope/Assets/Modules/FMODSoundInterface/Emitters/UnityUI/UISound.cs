namespace FMODSoundInterface.UnityUI
{
    internal static class UISound
    {
        public static class Button
        {
            public const string Tap = "event:/ui/button/tap";
            public const string Confirm = "event:/ui/button/confirm";
            public const string Cancel = "event:/ui/button/cancel";
        }

        public static class Slider
        {
            public const string Slide = "event:/ui/slider/slide";
        }

        public static class Dropdown
        {
            public const string Expand = "event:/ui/dropdown/expand";
            public const string Collapse = "event:/ui/dropdown/collapse";
            public const string Select = "event:/ui/dropdown/select";
        }

        public static class InputField
        {
            public const string Blur = "event:/ui/inputfield/blur";
            public const string Change = "event:/ui/inputfield/change";
        }

        public static class Toggle
        {
            public const string Change = "event:/ui/toggle/change";
            public const string Enabled = "event:/ui/toggle/enabled";
            public const string Disabled = "event:/ui/toggle/disabled";
        }
    }
}