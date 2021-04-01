using AnimatedObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DopeElections.Planets
{
    public class CloudController : MonoBehaviour
    {
        [SerializeField] private Transform _transform = null;
        [SerializeField] private Transform _meshTransform = null;
        [SerializeField] private ToggleableObjectController _animationController = null;
        [SerializeField] private ParticleSystem.MinMaxCurve _velocity = default;
        [SerializeField] private ParticleSystem.MinMaxCurve _size = default;

        private float _speed;

        private void Awake()
        {
            _animationController.Show();
        }

        private void Start()
        {
            _speed = _velocity.Evaluate(0, Random.value);
            _meshTransform.localScale = Vector3.one * _size.Evaluate(0, Random.value);
        }

        private void Update()
        {
            var rotation = Quaternion.Euler(Time.deltaTime * _speed, 0, 0);
            _transform.rotation *= rotation;
        }

        public void Remove()
        {
            _animationController.Hide().Then(() => Destroy(gameObject));
        }
    }
}