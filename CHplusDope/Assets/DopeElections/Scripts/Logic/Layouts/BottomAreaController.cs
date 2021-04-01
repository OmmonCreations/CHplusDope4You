using UnityEngine;
using UnityEngine.EventSystems;

namespace DopeElections.Layouts
{
    [ExecuteAlways]
    public class BottomAreaController : UIBehaviour
    {
        [SerializeField] private RectTransform _instructionPanelTransform = null;
        [SerializeField] private RectTransform _actionButtonTransform = null;
        [SerializeField] private TMP_TextSizeCalculator _instructionPanelText = null;
        [SerializeField] private TMP_TextSizeCalculator _actionButtonText = null;
        [SerializeField] private float _spacing = 36;

        protected override void OnEnable()
        {
            base.OnEnable();
            TriggerUpdate();
        }

        protected override void Start()
        {
            base.Start();
            TriggerUpdate();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            TriggerUpdate();
        }

        public void TriggerUpdate()
        {
            var instructionPanelPresent =
                _instructionPanelTransform && _instructionPanelTransform.gameObject.activeSelf;
            var actionButtonPresent =
                _actionButtonTransform && _actionButtonTransform.gameObject.activeSelf;

            if (instructionPanelPresent && actionButtonPresent) ApplyCombinedLayout();
            else if (instructionPanelPresent) ApplyInstructionPanelOnlyLayout();
            else if (actionButtonPresent) ApplyActionButtonOnlyLayout();
        }

        private void ApplyCombinedLayout()
        {
            if (!_instructionPanelTransform || !_instructionPanelText || !_actionButtonTransform || !_actionButtonText)
            {
                return;
            }

            var rectTransform = GetComponent<RectTransform>();
            var rect = rectTransform.rect;
            var width = rect.size.x;

            var instructionPanelRectTransform = _instructionPanelTransform;
            var actionButtonRectTransform = _actionButtonTransform;

            var instructionPanelWidth = _instructionPanelText.GetPreferredWidth();
            var actionButtonWidth = _actionButtonText.GetPreferredWidth();

            actionButtonRectTransform.anchorMin = new Vector2(1, 0);
            actionButtonRectTransform.anchorMax = new Vector2(1, 1);
            actionButtonRectTransform.offsetMin = new Vector2(-actionButtonWidth, 0);
            actionButtonRectTransform.offsetMax = Vector2.zero;

            var remainingRightHalfWidth = width / 2 - (actionButtonWidth + _spacing);
            var instructionPanelRightWidth = Mathf.Min(remainingRightHalfWidth, instructionPanelWidth / 2);
            var instructionPanelLeftWidth = Mathf.Min(instructionPanelWidth - instructionPanelRightWidth, width / 2);
            instructionPanelRectTransform.anchorMin = new Vector2(0.5f, 0);
            instructionPanelRectTransform.anchorMax = new Vector2(0.5f, 1);
            instructionPanelRectTransform.offsetMin = new Vector2(-instructionPanelLeftWidth, 0);
            instructionPanelRectTransform.offsetMax = new Vector2(instructionPanelRightWidth, 0);
        }

        private void ApplyInstructionPanelOnlyLayout()
        {
            if (!_instructionPanelTransform || !_instructionPanelText) return;
            var rectTransform = _instructionPanelTransform;
            var preferredWidth = _instructionPanelText.GetPreferredWidth();

            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.offsetMin = new Vector2(-preferredWidth / 2, 0);
            rectTransform.offsetMax = new Vector2(preferredWidth / 2, 0);
        }

        private void ApplyActionButtonOnlyLayout()
        {
            if (!_actionButtonTransform || !_actionButtonText) return;
            var rectTransform = _actionButtonTransform;
            var preferredWidth = _actionButtonText.GetPreferredWidth();
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.offsetMin = new Vector2(-preferredWidth / 2, 0);
            rectTransform.offsetMax = new Vector2(preferredWidth / 2, 0);
        }
    }
}