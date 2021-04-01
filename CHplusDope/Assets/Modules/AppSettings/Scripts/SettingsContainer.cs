using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AppSettings
{
    public class SettingsContainer: IEnumerable<KeyValuePair<SettingType,SettingValue>>
    {
        private readonly Dictionary<SettingType,SettingValue> _values = new Dictionary<SettingType,SettingValue>();

        public string File { get; }
        
        public SettingsContainer(string file,  params SettingType[] entries) : this(file, (IEnumerable<SettingType>) entries)
        {
            
        }
        
        public SettingsContainer(string file, IEnumerable<SettingType> entries)
        {
            File = file;
            
            foreach (var type in entries)
            {
                _values.Add(type, type.CreateValue());
            }
        }
        
        public T2 GetValue<T2>(SettingType<T2> setting)
        {
            return _values.TryGetValue(setting, out var v) && v is SettingValue<T2> result ? result.Value : setting.DefaultValue;
        }

        public void SetValue<T2>(SettingType<T2> setting, T2 value)
        {
            var valueEntry = _values.TryGetValue(setting, out var v) && v is SettingValue<T2> sv ? sv : default;
            if (valueEntry == default) return;
            valueEntry.Value = value;
        }

        public void ApplyValues()
        {
            foreach (var entry in _values)
            {
                entry.Key.Apply(entry.Value);
            }
        }

        public void SetDefaultValues()
        {
            foreach (var entry in _values)
            {
                entry.Key.SetDefaultValue(entry.Value);
            }
        }

        public void Load(JObject data)
        {
            foreach (var entry in _values)
            {
                var key = entry.Key;
                if (data[key.Key] == null)
                {
                    key.SetDefaultValue(entry.Value);
                    continue;
                }
                
                entry.Value.Deserialize(data[key.Key]);
            }
        }

        public JObject Save()
        {
            var result = new JObject();
            foreach (var entry in _values)
            {
                result[entry.Key.Key] = entry.Value.Serialize();
            }

            return result;
        }

        public IEnumerator<KeyValuePair<SettingType, SettingValue>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}