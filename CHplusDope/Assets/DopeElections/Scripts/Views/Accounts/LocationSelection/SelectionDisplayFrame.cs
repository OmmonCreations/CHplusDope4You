using DopeElections.Answer;
using DopeElections.Placeholders;
using StateMachines;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Accounts
{
    public class SelectionDisplayFrame : MonoBehaviour
    {
        [SerializeField] private RectTransform _bodyTransform = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Image _coatOfArms = null;
        [SerializeField] private StateMachine _stateMachine = null;

        private Canton _canton;
        private Constituency _constituency;

        public float Alpha
        {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }

        private RectTransform BodyTransform => _bodyTransform;
        private StateMachine StateMachine => _stateMachine;

        private void Update()
        {
            StateMachine.Run();
        }

        public void Set(Canton canton, Constituency constituency)
        {
            if (canton == _canton && constituency == _constituency)
            {
                if (canton == null || constituency == null) gameObject.SetActive(false);
                return;
            }

            _coatOfArms.enabled = false;
            if (canton != null && !string.IsNullOrWhiteSpace(canton.coatOfArms))
            {
                WebSprite.LoadSquare(canton.coatOfArms).Assign(s =>
                {
                    if (!_coatOfArms) return;
                    _coatOfArms.sprite = s;
                    _coatOfArms.enabled = s;
                });
            }

            _canton = canton;
            _constituency = constituency;

            if (canton != null && constituency != null) FadeIn();
            else gameObject.SetActive(false);
        }

        public void Clear()
        {
            _coatOfArms.enabled = false;
            _coatOfArms.sprite = null;
            _canton = null;
            _constituency = null;
            Alpha = 0;
            gameObject.SetActive(false);
        }

        private void FadeIn()
        {
            Alpha = 0;
            var transition = new TransitionState(0.2f, 0, 1);
            var fromOpacity = 0;
            var toOpacity = 1;
            var fromSize = Vector3.one * 1.3f;
            var toSize = Vector3.one;
            transition.OnTransition += t =>
            {
                var progress = 1 - Mathf.Pow(1 - t, 2);
                Alpha = Mathf.Lerp(fromOpacity, toOpacity, t);
                BodyTransform.localScale = Vector3.Lerp(fromSize, toSize, progress);
            };
            transition.OnCompleted += () =>
            {
                Alpha = 1;
                BodyTransform.localScale = Vector3.one;
            };
            gameObject.SetActive(true);
            StateMachine.State = transition;
        }
    }
}