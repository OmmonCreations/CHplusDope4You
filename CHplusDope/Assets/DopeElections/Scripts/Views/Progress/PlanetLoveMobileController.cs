using DopeElections.LoveMobiles;
using OpenSimplexNoiseAlgorithm;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DopeElections.Progress
{
    public class PlanetLoveMobileController : MonoBehaviour
    {
        [SerializeField] private LoveMobileController _loveMobile = null;
        [SerializeField] private Transform _pivot = null;
        [SerializeField] private Vector3 _axis = Vector3.right;
        [SerializeField] private Vector3 _offset = Vector3.zero;
        [SerializeField] private float _minAngle = 0;
        [SerializeField] private float _maxAngle = 180;
        [SerializeField] private float _delay = 0.3f;
        [SerializeField] private float _swerveFrequency = 0.1f;
        [SerializeField] private float _swayStrength = 5f;

        private bool _canMove = true;
        private OpenSimplexNoise _noise = null;
        private float _currentPosition;
        private float _targetPosition;
        private float _velocity;
        private bool _interactable;

        public bool Interactable
        {
            get => _interactable;
            set => ApplyInteractable(value);
        }

        private ProgressionView View { get; set; }

        public void Initialize(ProgressionView view)
        {
            View = view;
            _noise = new OpenSimplexNoise(Random.Range(0, int.MaxValue));
            _loveMobile.Pressed += OnLoveMobilePressed;
        }

        void Update()
        {
            if (_canMove && Mathf.Abs(_targetPosition - _currentPosition) > 0)
            {
                var position = Mathf.SmoothDamp(_currentPosition, _targetPosition, ref _velocity, _delay);
                ApplyAngle(position);
            }
        }

        public void SetPosition(float normalizedPosition)
        {
            _targetPosition = Mathf.Lerp(_minAngle, _maxAngle, normalizedPosition);
        }

        public void SetPositionImmediate(float normalizedPosition)
        {
            ApplyAngle(Mathf.Lerp(_minAngle, _maxAngle, normalizedPosition));
        }

        public void EnableMovement()
        {
            _canMove = true;
        }

        public void DisableMovement()
        {
            _targetPosition = _currentPosition;
            _velocity = 0;
            _canMove = false;
        }

        private void OnLoveMobilePressed()
        {
            View.GoToLeaderboard();
        }

        private void ApplyInteractable(bool interactable)
        {
            _loveMobile.Interactable = interactable;
            _interactable = interactable;
        }

        private void ApplyAngle(float angle)
        {
            var swerve = _noise.Evaluate(0, angle / _swerveFrequency);
            _pivot.localEulerAngles = _axis * angle + _offset;
            _loveMobile.MeshTransform.localEulerAngles = new Vector3(0, 0, swerve * _swayStrength);
            _currentPosition = angle;
            _targetPosition = angle;
        }
    }
}