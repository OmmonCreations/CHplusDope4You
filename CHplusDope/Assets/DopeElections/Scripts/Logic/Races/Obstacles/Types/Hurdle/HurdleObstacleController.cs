using FMODSoundInterface;
using UnityEngine;

namespace DopeElections.Races.Hurdle
{
    public class HurdleObstacleController : RaceObstacleController<HurdleObstacle>
    {
        [SerializeField] private Animator _animator = null;

        public override void PlayAppearAnimation()
        {
            _animator.Play("appear");
        }

        public void PlayDroppedAnimation()
        {
            SoundController.Play(Obstacle.Type.DroppedSound);
        }

        public void PlayClearedAnimation()
        {
            SoundController.Play(Obstacle.Type.ClearedSound);
        }
    }
}