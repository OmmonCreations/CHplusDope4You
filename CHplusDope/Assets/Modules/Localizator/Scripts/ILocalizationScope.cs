namespace Localizator
{
    public interface ILocalizationScope
    {
        ILocalization Localization { get; }
        void UpdateLabels();
        void Add(ILocalizable l);
        void Remove(ILocalizable t);
    }
}