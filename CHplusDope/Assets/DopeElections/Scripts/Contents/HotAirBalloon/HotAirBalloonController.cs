using UnityEngine;

namespace DopeElections.HotAirBalloon
{
    public class HotAirBalloonController : MonoBehaviour
    {
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _characterAnchor = null;

        public Transform CharacterAnchor => _characterAnchor;

        public void PlayIdleAnimation(float fade = 0.8f)
        {
            _animator.CrossFadeInFixedTime("idle", fade);
        }

        public void PlaySwayAnimation(float fade = 0.8f)
        {
            _animator.CrossFadeInFixedTime("sway-slow", fade);
        }

        public void PlayMoveFastAnimation(float fade = 0.8f)
        {
            _animator.CrossFadeInFixedTime("move-fast", fade);
        }
    }
}