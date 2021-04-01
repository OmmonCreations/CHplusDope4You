using AppSettings;

namespace DopeElections
{
    public class EulaSetting : BooleanSetting
    {
        public EulaSetting() : base("eula_accepted", false)
        {
            
        }
    }
}