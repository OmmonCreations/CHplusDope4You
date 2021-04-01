using System;
using UnityEngine;

namespace AnimatedObjects.Materials
{
    public abstract class ToggleableMaterialController : ToggleableObjectController
    {
        [SerializeField] private Renderer _renderer = null;
        [SerializeField] private int[] _materialIndices = new[] {0};

        private Material[] _materials;
        
        public Material[] Materials
        {
            get => GetMaterials();
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            if (_materials != null)
            {
                foreach (var material in _materials)
                {
                    Destroy(material);
                }
            }
        }

        private Material[] GetMaterials()
        {
            if (_materials != null) return _materials;
            var indices = _materialIndices;
            var materials = new Material[indices.Length];

            var sharedMaterials = _renderer.sharedMaterials;
            
            for (var i = 0; i < indices.Length; i++)
            {
                materials[i] = Instantiate(sharedMaterials[indices[i]]);
            }

            _renderer.sharedMaterials = materials;

            _materials = materials;
            return materials;
        }
    }
}