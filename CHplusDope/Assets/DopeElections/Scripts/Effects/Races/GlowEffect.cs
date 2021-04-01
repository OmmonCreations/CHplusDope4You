using Effects;
using UnityEngine;

namespace DopeElections.Races
{
    public class GlowEffect : EffectInstance
    {
        private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");
        
        [SerializeField] private Renderer _renderer = null;
        [SerializeField] private Color _color = Color.white;

        private Material _material;
        private Transform _transform;

        private Camera _camera;
        private Transform _cameraTransform;

        public Color Color
        {
            get => _color;
            set => ApplyColor(value);
        }

        public Camera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                _cameraTransform = value.transform;
                _transform = transform;
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform) _transform.rotation = _cameraTransform.rotation;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_material) Destroy(_material);
        }

        public override void Pause()
        {
        }

        public override void Resume()
        {
        }

        private void ApplyColor(Color color)
        {
            if (color == _color) return;
            
            var material = _renderer.sharedMaterial;
            if (!_material)
            {
                material = Instantiate(_renderer.sharedMaterial);
                _renderer.sharedMaterial = material;
                _material = material;
            }

            _color = color;
            material.SetColor(EmissionColorProperty, color);
            if (color.a <= 0 && gameObject.activeSelf) gameObject.SetActive(false);
            else if (color.a > 0 && !gameObject.activeSelf) gameObject.SetActive(true);
        }
    }
}