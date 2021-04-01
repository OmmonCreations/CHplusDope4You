using Essentials;
using StateMachines;
using UnityEngine;
using UnityEngine.UI;

namespace Effects
{
    public class CanvasPopEffect : CanvasEffectInstance<CanvasPopEffect.CanvasPopEffectData>
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private StateMachine _stateMachine = null;

        public StateMachine StateMachine => _stateMachine;

        protected override void OnPlay()
        {
            base.OnPlay();
            StateMachine.State = new PopState(this);
            RectTransform.Fill();
        }

        protected override void Update()
        {
            base.Update();
            StateMachine.Run();
        }

        public override void Pause()
        {
            enabled = false;
        }

        public override void Resume()
        {
            enabled = true;
        }

        public class CanvasPopEffectData : EffectData
        {
            public RectTransform Content { get; }
            
            public float AnimationTime { get; set; } = 0.5f;
            public float StartSize { get; set; } = 1;
            public float FinalSize { get; set; } = 2;
            public AnimationCurve OpacityCurve { get; set; } = AnimationCurve.Linear(0,1,1,0);
            public AnimationCurve SizeCurve { get; set; } = AnimationCurve.Linear(0,0,1,1);

            public CanvasPopEffectData(RectTransform content)
            {
                Content = content;
            }
        }
        
        private class PopState : State
        {
            private CanvasPopEffect EffectInstance { get; }
            
            private RectTransform RectTransform { get; }
            private CanvasGroup CanvasGroup { get; }

            private float AnimationTime { get; }
            private float StartSize { get; }
            private float FinalSize { get; }
            
            private AnimationCurve OpacityCurve { get; }
            private AnimationCurve SizeCurve { get; }
            
            private float _t;
            
            public PopState(CanvasPopEffect effectInstance)
            {
                EffectInstance = effectInstance;
                RectTransform = effectInstance.RectTransform;
                CanvasGroup = effectInstance._canvasGroup;

                StartSize = effectInstance.Data.StartSize;
                FinalSize = effectInstance.Data.FinalSize;

                OpacityCurve = effectInstance.Data.OpacityCurve;
                SizeCurve = effectInstance.Data.SizeCurve;

                AnimationTime = effectInstance.Data.AnimationTime;
            }

            protected override void OnInitialize()
            {
                base.OnInitialize();
                var template = EffectInstance.Data.Content;
                var contentInstance = Instantiate(template, RectTransform, false);
                contentInstance.Fill();
                contentInstance.gameObject.SetActive(true);
                if (template.GetComponent<LayoutGroup>() != null)
                {
                    RectTransform.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
                }

                RectTransform.SetParent(template, false);
                RectTransform.Fill();
            }

            public override void Update()
            {
                _t += Time.deltaTime / AnimationTime;

                var opacity = OpacityCurve.Evaluate(_t);
                var size = SizeCurve.Evaluate(_t);

                RectTransform.localScale = Vector3.one * Mathf.Lerp(StartSize, FinalSize, size);
                CanvasGroup.alpha = opacity;
                
                if (_t >= 1) IsCompleted = true;
            }

            protected override void OnComplete()
            {
                base.OnComplete();
                EffectInstance.Remove();
            }
        }
    }
}