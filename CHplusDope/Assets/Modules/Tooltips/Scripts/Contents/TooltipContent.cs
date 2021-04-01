using Essentials;

namespace Tooltips
{
    public abstract class TooltipContent
    {
        public abstract NamespacedKey TypeId { get; }

        public static class Builtin
        {
            private const string BuiltinNamespace = "builtin";
            public static NamespacedKey Text => new NamespacedKey(BuiltinNamespace, "text");
            public static NamespacedKey Separator => new NamespacedKey(BuiltinNamespace, "separator");
        }
    }
}