using Newtonsoft.Json.Linq;

namespace AppSettings
{
    public class BooleanValue : SettingValue<bool>
    {
        public override JToken Serialize()
        {
            return Value;
        }

        public override void Deserialize(JToken json)
        {
            Value = (bool) json;
        }
    }
}