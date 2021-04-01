using AnimatedObjects;
using Html;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections
{
    public class HtmlViewComponents : MonoBehaviour
    {
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Button _closeBackground = null;
        [SerializeField] private HtmlCanvas _htmlCanvas = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private ToggleableObjectController _animationController = null;

        public Button CloseButton => _closeButton;
        public Button CloseBackground => _closeBackground;
        public HtmlCanvas HtmlCanvas => _htmlCanvas;
        public LocalizedText ConfirmText => _confirmText;
        public ScrollRect ScrollRect => _scrollRect;
        public ToggleableObjectController AnimationController => _animationController;
    }
}