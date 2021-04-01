using Newtonsoft.Json.Linq;

namespace AppSettings
{
    public class FloatValue : SettingValue<float>
    {
        public override JToken Serialize()
        {
            return Value;
        }

        public override void Deserialize(JToken json)
        {
            Value = (float) json;
        }
    }
}