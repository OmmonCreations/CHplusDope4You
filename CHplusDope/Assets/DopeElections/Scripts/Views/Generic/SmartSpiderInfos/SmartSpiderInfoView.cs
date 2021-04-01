using DopeElections.Localizations;
using Essentials;
using Localizator;

namespace DopeElections.SmartSpiderInfos
{
    public class SmartSpiderInfoView : AbstractTutorialView
    {
        public override NamespacedKey Id => GenericViewId.SmartSpiderInfo;

        protected override LocalizationKey ConfirmKey => LKey.Views.SmartSpiderInfo.Confirm;

        protected override string GetHtmlFile(string languageCode)
        {
            return $"texts/{languageCode}/text-smartSpider-{languageCode}.html";
        }
    }
}