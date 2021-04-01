using System;

namespace AppSettings
{
    public abstract class SettingType
    {
        public string Key { get; }

        protected SettingType(string key)
        {
            Key = key;
        }

        internal abstract SettingValue CreateValue();
        
        /// <summary>
        /// Applies the supplied value to the environment
        /// </summary>
        public abstract void Apply(SettingValue value);
        
        /// <summary>
        /// Applies the default value to the supplied value instance
        /// </summary>
        public abstract void SetDefaultValue(SettingValue value);
    }

    public abstract class SettingType<T> : SettingType
    {
        public abstract T DefaultValue { get; }

        public SettingType(string key) : base(key)
        {
            
        }

        internal override SettingValue CreateValue()
        {
            return CreateValueInstance();
        }

        protected abstract SettingValue<T> CreateValueInstance();

        public override void SetDefaultValue(SettingValue value)
        {
            if (!(value is SettingValue<T> v)) throw new InvalidCastException("Cannot apply value of type "+GetType().Name+" to value instance of type "+value.GetType().Name+"!");
            v.Value = DefaultValue;
        }

        public override void Apply(SettingValue value)
        {
            if(!(value is SettingValue<T> v)) throw new InvalidCastException("Cannot apply value of type "+value.GetType().Name+" to setting of type "+GetType().Name+"!");
            Apply(v.Value);
        }

        public virtual void Apply(T value)
        {
            
        }
    }
}