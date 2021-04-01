using AnimatedObjects;
using StateMachines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DopeElections.Planets
{
    public class PlanetController : MonoBehaviour
    {
        [SerializeField] private ToggleableObjectController _animationController = null;
        [SerializeField] private Transform _meshTransform = null;
        [SerializeField] private Transform _rotationAxis = null;
        [SerializeField] private bool _rotate = true;
        [SerializeField] private bool _reverseRotation = true;
        [SerializeField] private float _rotationSpeed = 10;
        [SerializeField] private ToggleableObjectController _cloudLayer = null;
        [SerializeField] private Transform _cloudsAnchor = null;
        [SerializeField] private CloudController[] _cloudPrefabs = null;

        private CloudController[] _clouds;

        private ToggleableObjectController AnimationController => _animationController;
        public ToggleableObjectController CloudLayer => _cloudLayer;
        public Transform MeshTransform => _meshTransform;

        public bool AutoRotate
        {
            get => _rotate;
            set => _rotate = value;
        }

        private void Update()
        {
            if (_rotate)
            {
                _meshTransform.Rotate(_rotationAxis.up, Time.deltaTime * _rotationSpeed * (_reverseRotation ? -1 : 1));
            }
        }

        public TransitionState Show() => Show(true);
        public TransitionState Hide() => Show(false);

        public TransitionState Show(bool show)
        {
            gameObject.SetActive(true);
            var result = AnimationController.Show(show);
            if (!show) result.OnCompleted += () => gameObject.SetActive(false);
            return result;
        }

        public void ShowImmediate() => ShowImmediate(true);
        public void HideImmediate() => ShowImmediate(false);

        public void ShowImmediate(bool show)
        {
            _animationController.ShowImmediate(show);
        }

        private void ClearClouds()
        {
            if (_clouds == null) return;
            foreach (var c in _clouds) c.Remove();
        }

        public void SpawnClouds(int count = 15)
        {
            ClearClouds();
            var clouds = new CloudController[count];
            for (var i = 0; i < count; i++)
            {
                clouds[i] = CreateCloud();
            }

            _clouds = clouds;
        }

        private CloudController CreateCloud()
        {
            var instanceObject = Instantiate(_cloudPrefabs[Random.Range(0, _cloudPrefabs.Length)].gameObject,
                _cloudsAnchor, false);
            var instance = instanceObject.GetComponent<CloudController>();
            var transform = instanceObject.transform;
            var eulerAngles = new Vector3(
                Random.value * 360,
                Random.value * 360,
                Random.value * 360
            );
            transform.localEulerAngles = eulerAngles;
            instanceObject.SetActive(true);
            return instance;
        }
    }
}