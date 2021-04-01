using System.Collections.Generic;
using System.Linq;

namespace Localizator
{
    public static class LocalizationUtility
    {
        public static string ApplyReplacements(string s, IEnumerable<KeyValuePair<string, string>> replacements)
        {
            return replacements.Aggregate(s, (current, e) => current.Replace("{" + e.Key + "}", e.Value));
        }
    }
}