using UnityEngine;

namespace Localizator
{
    [System.Serializable]
    public readonly struct Language
    {
        public static readonly Language german = new Language("de", "Deutsch", SystemLanguage.German);
        public static readonly Language english = new Language("en", "English", SystemLanguage.English);
        public static readonly Language french = new Language("fr", "Français", SystemLanguage.French);

        public readonly string code;
        public readonly string name;
        public readonly SystemLanguage systemLanguage;

        public Language(string code, string name, SystemLanguage systemLanguage = SystemLanguage.Unknown)
        {
            this.code = code;
            this.name = name;
            this.systemLanguage = systemLanguage;
        }

        public bool Equals(Language other)
        {
            return code == other.code;
        }

        public override bool Equals(object obj)
        {
            return obj is Language other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (code != null ? code.GetHashCode() : 0);
        }
        
        public static bool operator ==(Language a, Language b)
        {
            return a.code == b.code;
        }

        public static bool operator !=(Language a, Language b)
        {
            return !(a == b);
        }
    }
}