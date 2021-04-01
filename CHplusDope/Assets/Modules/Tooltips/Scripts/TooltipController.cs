using System.Collections.Generic;
using System.Linq;
using Localizator;
using StateMachines;
using UnityEngine;

namespace Tooltips
{
    public abstract class TooltipController : MonoBehaviour
    {
        public delegate void TooltipEvent();

        public event TooltipEvent OnRemove;

        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private RectTransform _contentArea = null;
        [SerializeField] private float _maxWidth = 400;

        private TooltipContentController[] _templates;
        private Tooltip _tooltip;
        private readonly List<TooltipContentController> _content = new List<TooltipContentController>();

        public T GetContent<T>(string id = null) where T : TooltipContentController =>
            _content.OfType<T>().FirstOrDefault(c => id == null || c.Id == id);

        private ITargetable _target;

        public TooltipsController TooltipsController { get; private set; }

        public RectTransform RectTransform => _rectTransform;
        public RectTransform TooltipsLayer { get; private set; }
        public TooltipContentController[] Content => _content.ToArray();
        public virtual ITargetable Target { get; set; }
        public ILocalization Localization => TooltipsController.Localization;

        public Tooltip Tooltip
        {
            get => _tooltip;
            set => ApplyTooltip(value);
        }
        
        public float MaxWidth
        {
            get => _maxWidth;
            set => _maxWidth = value;
        }

        protected virtual void Start()
        {
            RecalculateLayout();
        }

        protected virtual void OnDestroy()
        {
            if (OnRemove != null) OnRemove();
        }

        internal void Initialize(TooltipsController tooltipController, RectTransform tooltipsLayer)
        {
            TooltipsController = tooltipController;
            TooltipsLayer = tooltipsLayer;
            RectTransform.anchorMin = Vector2.up;
            RectTransform.anchorMax = Vector2.up;
            RectTransform.pivot = Vector2.up;
        }

        private void ApplyTooltip(Tooltip tooltip)
        {
            foreach (var previous in _content)
            {
                previous.Remove();
            }

            _tooltip = tooltip;
            _content.Clear();
            var templates = TooltipsController.Templates;
            foreach (var entry in tooltip.Contents)
            {
                var id = entry.Key;
                var content = entry.Value;
                var template = templates.FirstOrDefault(t => t.TypeId == content.TypeId);
                if (!template || template == null)
                {
                    Debug.LogError("Tooltip content template with type id " + content.TypeId + " missing!");
                    continue;
                }

                var instanceObject = Instantiate(template.gameObject, _contentArea, false);
                var instance = instanceObject.GetComponent<TooltipContentController>();
                instance.Initialize(this, id, content);
                _content.Add(instance);
            }

            _templates = templates;
        }

        public TooltipContentController AddContent<T>(string id, T content) where T : TooltipContent
        {
            var template = _templates.FirstOrDefault(t => t.TypeId == content.TypeId);
            if (!template || template == null)
            {
                Debug.LogError("Tooltip content template with type id " + content.TypeId + " missing!");
                return null;
            }
            var instanceObject = Instantiate(template.gameObject, _contentArea, false);
            var instance = instanceObject.GetComponent<TooltipContentController>();
            instance.Initialize(this, id, content);
            _content.Add(instance);
            _tooltip[id] = content;
            return instance;
        }

        internal void Remove(TooltipContentController content)
        {
            _content.Remove(content);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        /*
        protected abstract float GetWidth();
        protected abstract float GetHeight();
        protected abstract Vector2 GetOffset();
        */

        public void RecalculateLayout()
        {
            foreach (var content in _content)
            {
                //content.UpdateLabels();
            }
        }

        protected void GoToMousePosition()
        {
            SetPosition(Input.mousePosition);
        }

        protected void SetPosition(Vector2 screenPosition)
        {
            RectTransform.anchoredPosition = screenPosition;
        }
    }
}