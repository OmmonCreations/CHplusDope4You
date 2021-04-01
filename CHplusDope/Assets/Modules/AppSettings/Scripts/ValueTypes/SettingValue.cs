using Newtonsoft.Json.Linq;

namespace AppSettings
{
    public abstract class SettingValue
    {
        public abstract JToken Serialize();

        public abstract void Deserialize(JToken json);
    }
    
    public abstract class SettingValue<T> : SettingValue
    {
        public delegate void ValueChangedEvent(T value);

        public event ValueChangedEvent ValueChanged = delegate{};
        
        private T _value;
        public T Value
        {
            get => _value;
            set => ApplyValue(value);
        }

        private void ApplyValue(T value)
        {
            _value = value;
            OnValueChanged(value);
            ValueChanged(value);
        }

        protected virtual void OnValueChanged(T value)
        {
            
        }
    }
}