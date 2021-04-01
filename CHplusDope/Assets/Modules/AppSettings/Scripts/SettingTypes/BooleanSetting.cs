namespace AppSettings
{
    public class BooleanSetting : SettingType<bool>
    {
        public override bool DefaultValue { get; }
        
        public BooleanSetting(string key, bool defaultValue) : base(key)
        {
            DefaultValue = defaultValue;
        }

        protected override SettingValue<bool> CreateValueInstance()
        {
            return new BooleanValue();
        }
    }
}