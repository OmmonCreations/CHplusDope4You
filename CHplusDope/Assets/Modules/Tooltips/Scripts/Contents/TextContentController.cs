using Essentials;
using Localizator;
using UnityEngine;

namespace Tooltips
{
    public class TextContentController : TooltipContentController<TextContent>
    {
        public override NamespacedKey TypeId => TooltipContent.Builtin.Text;

        [SerializeField] private LocalizedText _text = null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _text.key = Content.Key;
        }
    }
}