using System;
using AnimatedObjects;
using DopeElections.Progression;
using DopeElections.Questions;
using Progression;
using UnityEngine;

namespace DopeElections.Progress
{
    public class ProgressStepMeshController : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;

        [SerializeField] private Renderer _renderer = null;
        [SerializeField] private int _lampsIndex = 2;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private Material _activeMaterial = null;
        [SerializeField] private Material _passiveMaterial = null;
        [SerializeField] private ParticleSystem _unlockParticles = null;

        private bool _locked;
        private bool _isActive;
        private string _animation;
        private bool _animatioPlayed = false;

        public bool IsActive
        {
            get => _isActive;
            set => ApplyActive(value);
        }

        public bool Locked
        {
            get => _locked;
            set => ApplyLocked(value);
        }

        private void OnEnable()
        {
            UpdateAnimation();
        }

        private void ApplyActive(bool active)
        {
            _isActive = active;
            var materials = _renderer.sharedMaterials;
            if (_lampsIndex >= materials.Length) return;
            materials[_lampsIndex] = active ? _activeMaterial : _passiveMaterial;
            _renderer.sharedMaterials = materials;
        }

        private void ApplyLocked(bool locked)
        {
            _locked = locked;
            _animation = locked ? "idle-locked" : "idle";
            _animatioPlayed = false;
            UpdateAnimation();
        }

        public void PlayUnlockAnimation()
        {
            _animator.Play("unlock");
        }

        public void PlayUnlockParticles()
        {
            _unlockParticles.Play();
        }

        public void UpdateIcon(IVisibleProgressEntry entry)
        {
            var icon = GetIcon(entry);

            _spriteRenderer.sprite = icon;
            _spriteRenderer.enabled = icon;
        }

        private Sprite GetIcon(IVisibleProgressEntry entry)
        {
            if (entry is RaceCategoryProgressEntry categoryEntry && categoryEntry.Configuration != 0)
            {
                var category = categoryEntry.GetCategory();
                return category != null ? category.GetIconWhite() : null;
            }

            return null;
        }

        private void UpdateAnimation()
        {
            if (!gameObject.activeInHierarchy || _animatioPlayed) return;
            _animator.Play(_animation);
            _animatioPlayed = true;
        }
    }
}