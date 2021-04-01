using AppSettings;
using Newtonsoft.Json.Linq;

namespace Localizator.Settings
{
    public class LanguageValue : SettingValue<string>
    {
        public override JToken Serialize()
        {
            return Value;
        }

        public override void Deserialize(JToken json)
        {
            Value = json != null ? (string) json : null;
        }
    }
}