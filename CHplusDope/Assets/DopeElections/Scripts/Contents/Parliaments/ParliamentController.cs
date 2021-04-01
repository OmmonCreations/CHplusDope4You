using UnityEngine;

namespace DopeElections.Parliaments
{
    public class ParliamentController : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;

        private bool _locked;

        public bool Locked
        {
            get => _locked;
            set => ApplyLocked(value);
        }

        private void OnEnable()
        {
            UpdateState();
        }

        private void ApplyLocked(bool locked)
        {
            _locked = locked;
            UpdateState();
        }

        private void UpdateState()
        {
            if (!gameObject.activeInHierarchy) return;
            _animator.Play(Locked ? "locked" : "open");
        }
    }
}