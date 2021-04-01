using Essentials;
using Localizator;

namespace Tooltips
{
    public class TextContent : TooltipContent
    {
        public override NamespacedKey TypeId => Builtin.Text;

        public LocalizationKey Key { get; }
        
        public TextContent(LocalizationKey key)
        {
            Key = key;
        }

        public TextContent(string text)
        {
            Key = new LocalizationKey{fallback = text};
        }
    }
}