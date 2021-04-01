using Essentials;
using Localizator;
using UnityEngine;

namespace Tooltips
{
    public abstract class TooltipContentController : MonoBehaviour
    {
        public abstract NamespacedKey TypeId { get; }
        
        public string Id { get; private set; }
        public TooltipController Tooltip { get; private set; }
        public TooltipContent Content { get; private set; }
        protected ILocalization Localization => Tooltip.Localization;
        
        internal void Initialize(TooltipController tooltip, string id, TooltipContent content)
        {
            Tooltip = tooltip;
            Id = id;
            Content = content;
            OnInitialize();
        }
        
        protected virtual void OnInitialize(){}

        protected virtual void OnDestroy()
        {
            if(Tooltip) Tooltip.Remove(this);
        }
        
        public void Remove()
        {
            Destroy(gameObject);
        }
    }

    public abstract class TooltipContentController<T> : TooltipContentController where T : TooltipContent
    {
        public new T Content => base.Content as T;
    }
}