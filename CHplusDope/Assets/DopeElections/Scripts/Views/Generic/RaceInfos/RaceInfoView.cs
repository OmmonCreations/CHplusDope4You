using DopeElections.Localizations;
using Essentials;
using Localizator;

namespace DopeElections.RaceInfos
{
    public class RaceInfoView : AbstractTutorialView
    {
        public override NamespacedKey Id => GenericViewId.RaceInfo;

        protected override LocalizationKey ConfirmKey => LKey.Views.RaceInfo.Confirm;
        
        protected override string GetHtmlFile(string languageCode)
        {
            return $"texts/{languageCode}/text-race-{languageCode}.html";
        }
    }
}