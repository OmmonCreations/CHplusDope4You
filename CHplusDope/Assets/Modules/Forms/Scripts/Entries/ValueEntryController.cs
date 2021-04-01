using System;
using Newtonsoft.Json.Linq;

namespace Forms
{
    public abstract class ValueEntryController : FormEntryController<ValueEntry>
    {
        private bool _required;
        private JToken _value;
        private JToken _defaultValue;
        
        public string Key { get; private set; }
        
        public JToken Value
        {
            get => _value;
            set => ApplyValue(value);
        }

        public JToken DefaultValue => _defaultValue;

        protected override void ApplyEntry(ValueEntry entry)
        {
            Key = entry.Key;
            _defaultValue = entry.DefaultValue;
            _required = entry.Required;
            if(entry.Value!=null) ApplyValue(entry.Value);
        }

        protected virtual void ApplyValue(JToken value)
        {
            if ((value!=null && value.Equals(_value)) || (value==_value)) return;
            _value = value;
            Form.TriggerChange();
        }

        public override void ApplyDefaults()
        {
            base.ApplyDefaults();
            Value = DefaultValue;
        }

        public override void SaveValues(JObject data)
        {
            base.SaveValues(data);
            if (Key == null) return;
            var parts = Key.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                var parentData = data;
                if (i == parts.Length - 1)
                {
                    parentData[part] = Value;
                    return;
                }
                
                data = parentData[part] as JObject;
                if (data != null) continue;
                data = new JObject();
                parentData[part] = data;
            }
        }

        public override bool Validate()
        {
            return (!_required || (Value!=null && !string.IsNullOrWhiteSpace(Value.ToString())));
        }
    }

    public abstract class ValueEntry<T> : ValueEntryController where T : ValueEntry
    {
        protected sealed override void ApplyEntry(ValueEntry entry)
        {
            if (!(entry is T t))
            {
                throw new InvalidOperationException("Cannot apply configuration of type " +
                                                    entry.GetType().Name + " to entry of type " +
                                                    GetType().Name + "!");
            }
            base.ApplyEntry(entry);
            ApplyEntry(t);
        }

        protected abstract void ApplyEntry(T entry);
    }
}