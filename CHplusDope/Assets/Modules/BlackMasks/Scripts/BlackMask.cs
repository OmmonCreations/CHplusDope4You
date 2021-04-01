using System;
using StateMachines;
using UnityEngine;

namespace BlackMasks
{
    /// <summary>
    /// Fades whole screen
    /// </summary>
    public class BlackMask : MonoBehaviour
    {
        public delegate void TransitionStartEvent(float time);

        public delegate void TransitionCompleteEvent();

        public event TransitionStartEvent FadesToBlack = delegate { };
        public event TransitionCompleteEvent FadedToBlack = delegate { };
        public event TransitionStartEvent FadesToClear = delegate { };
        public event TransitionCompleteEvent FadedToClear = delegate { };

        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private GameObject _canvasGroupObject = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private CanvasGroup _interactionBlocker = null;

        public float Alpha
        {
            get => _canvasGroup.alpha;
            set
            {
                _canvasGroup.alpha = value;
                if (value <= 0 && _canvasGroupObject.activeSelf) _canvasGroupObject.SetActive(false);
                else if (value > 0 && !_canvasGroupObject.activeSelf) _canvasGroupObject.SetActive(true);
                _canvasGroup.blocksRaycasts = value >= 1;
            }
        }

        public StateMachine StateMachine => _stateMachine;

        private void Awake()
        {
            BlockInteractions(false);
        }

        private void Update()
        {
            StateMachine.Run();
        }

        public void BlockInteractions(bool block)
        {
            _interactionBlocker.gameObject.SetActive(block);
        }

        public TransitionState FadeToBlack(Action callback = null) => FadeToBlack(0.2f, callback);

        public TransitionState FadeToBlack(float time, Action callback = null)
        {
            Alpha = 0;
            FadesToBlack(time);
            return StartTransition(1, time, () =>
            {
                FadedToBlack();
                if (callback != null) callback();
            });
        }

        public TransitionState FadeToClear(Action callback = null) => FadeToClear(0.2f, callback);

        public TransitionState FadeToClear(float time, Action callback = null)
        {
            Alpha = 1;
            FadesToClear(time);
            return StartTransition(0, time, () =>
            {
                FadedToClear();
                if (callback != null) callback();
            });
        }

        private TransitionState StartTransition(float targetAlpha, float time, Action callback)
        {
            var startAlpha = Alpha;
            var state = new TransitionState(time, 0, 1);
            state.OnTransition += t => Alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            if (callback != null) state.OnFinished += () => callback();
            StateMachine.State = state;
            return state;
        }
    }
}