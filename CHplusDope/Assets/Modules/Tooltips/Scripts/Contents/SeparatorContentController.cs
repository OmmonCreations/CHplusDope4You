using Essentials;

namespace Tooltips
{
    public class SeparatorContentController : TooltipContentController<SeparatorContent>
    {
        public override NamespacedKey TypeId => TooltipContent.Builtin.Separator;
    }
}