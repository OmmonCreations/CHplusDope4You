using StateMachines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopupInfos
{
    public class PopupInfoController : MonoBehaviour
    {
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private Image _image = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;

        [Header("Animation")] [SerializeField]
        private ParticleSystem.MinMaxCurve _popForce = new ParticleSystem.MinMaxCurve(20, 50);

        [SerializeField] private ParticleSystem.MinMaxCurve _gravity = new ParticleSystem.MinMaxCurve(50,
            new AnimationCurve(new Keyframe(0, 1, -1, 1), new Keyframe(1, 0, 0.5f, -0.5f)));

        [SerializeField] private ParticleSystem.MinMaxCurve _distance = new ParticleSystem.MinMaxCurve(0);
        [SerializeField] private ParticleSystem.MinMaxCurve _emissionAngle = new ParticleSystem.MinMaxCurve(-120, 120);
        [SerializeField] private ParticleSystem.MinMaxCurve _sizeAnimation = new ParticleSystem.MinMaxCurve(1);
        [SerializeField] private ParticleSystem.MinMaxCurve _alphaAnimation = new ParticleSystem.MinMaxCurve(1);

        private PopupInfoData _data;
        private float _size = 1;

        public PopupInfoLayer Layer { get; private set; }
        public ITargetable Target { get; private set; }

        public PopupInfoData Data
        {
            get => _data;
            set => ApplyData(value);
        }

        public RectTransform RectTransform => _rectTransform;
        public StateMachine StateMachine => _stateMachine;
        public ParticleSystem.MinMaxCurve PopForce => _popForce;
        public ParticleSystem.MinMaxCurve Gravity => _gravity;
        public ParticleSystem.MinMaxCurve Distance => _distance;
        public ParticleSystem.MinMaxCurve EmissionAngle => _emissionAngle;
        public ParticleSystem.MinMaxCurve SizeAnimation => _sizeAnimation;
        public ParticleSystem.MinMaxCurve AlphaAnimation => _alphaAnimation;

        public float Alpha
        {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }

        public float Size
        {
            get => _size;
            set => ApplySize(value);
        }

        internal void Initialize(PopupInfoLayer layer, ISpatialTargetable target)
        {
            Layer = layer;
            Target = target;
        }

        private void Update()
        {
            StateMachine.Run();
        }

        protected void OnDestroy()
        {
            if (Layer != null) Layer.Remove(this);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        private void ApplySize(float size)
        {
            _size = size;
            _rectTransform.localScale = Vector3.one * size;
        }

        private void ApplyData(PopupInfoData data)
        {
            _data = data;
            if (_text)
            {
                _text.text = data.Text;
                _text.color = data.Color;
                _text.enabled = data.Text != null;
            }

            if (_image)
            {
                _image.sprite = data.Sprite;
                _image.enabled = _image.sprite;
            }

            StateMachine.State = new PopTask(this);
            StateMachine.Run();
        }
    }
}