using StateMachines;
using UnityEngine;

namespace BlackMasks
{
    /// <summary>
    /// Fades whole screen except for a specified area
    /// </summary>
    public class BlackoutMask : MonoBehaviour
    {
        [SerializeField] private RectTransform _circleRectTransform = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private float _baseAlpha = 0.5f;

        private Vector2 _center;
        private Vector2 _size;
        private float _alpha = 1;
        private float _internalAlpha = 0;
        private bool _isVisible = false;

        public bool IsVisible => _isVisible;

        public StateMachine StateMachine => _stateMachine;

        public Vector2 Center
        {
            get => _center;
            set => ApplyCenter(value);
        }

        public Vector2 Size
        {
            get => _size;
            set => ApplySize(value);
        }
        
        public float Alpha
        {
            get => _baseAlpha;
            set => ApplyAlpha(value);
        }

        private void Update()
        {
            StateMachine.Run();
        }

        private void ApplyCenter(Vector2 center)
        {
            _center = center;
            _circleRectTransform.anchorMin = center;
            _circleRectTransform.anchorMax = center;
        }

        private void ApplySize(Vector2 size)
        {
            _size = size;
            _circleRectTransform.offsetMin = -size / 2;
            _circleRectTransform.offsetMax = size / 2;
        }

        private void ApplyAlpha(float alpha)
        {
            _alpha = alpha;
            UpdateAlpha();
        }

        private void ApplyInternalAlpha(float alpha)
        {
            _internalAlpha = alpha;
            UpdateAlpha();
        }

        private void UpdateAlpha()
        {
            _canvasGroup.alpha = _alpha * _baseAlpha * _internalAlpha;
        }
        
        public void Show(float time = 1)
        {
            _isVisible = true;
            
            gameObject.SetActive(true);
            if (time > 0)
            {
                var state = new TransitionState(time, 0, 1, _internalAlpha);
                state.OnTransition += ApplyInternalAlpha;
                state.OnCompleted += () => ApplyInternalAlpha(1);
                StateMachine.State = state;
            }
            else
            {
                ApplyInternalAlpha(1);
            }
        }

        public void Hide(float time = 1)
        {
            _isVisible = false;
            
            if (time > 0)
            {
                var state = new TransitionState(time, 1, 0, 1-_internalAlpha);
                state.OnTransition += ApplyInternalAlpha;
                state.OnCompleted += () =>
                {
                    ApplyInternalAlpha(0);
                    gameObject.SetActive(false);
                };
                StateMachine.State = state;
            }
            else
            {
                ApplyInternalAlpha(0);
                gameObject.SetActive(false);
            }
        }
    }
}