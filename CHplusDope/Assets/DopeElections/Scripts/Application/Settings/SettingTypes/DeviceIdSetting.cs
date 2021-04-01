using System;
using AppSettings;

namespace DopeElections
{
    public class DeviceIdSetting : SettingType<Guid>
    {
        public override Guid DefaultValue => Guid.NewGuid();
        
        public DeviceIdSetting(string key) : base(key)
        {
            
        }

        protected override SettingValue<Guid> CreateValueInstance()
        {
            return new GuidValue();
        }
    }
}