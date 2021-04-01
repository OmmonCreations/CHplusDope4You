using FMODSoundInterface;
using UnityEngine;

namespace DopeElections.ScriptedSequences.Splash
{
    public class CandidatesRunState : SplashCinematicState
    {
        private Vector3 From { get; }
        private Vector3 To { get; }
        private float Speed { get; }
        private float Randomization { get; }
        private float AnimationTime { get; }

        private float _t;

        public CandidatesRunState(SplashCinematicController controller, Vector3 from, Vector3 to, float speed,
            float randomization, float animationTime) :
            base(controller)
        {
            From = from;
            To = to;
            Speed = speed;
            Randomization = randomization;
            AnimationTime = animationTime;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Controller.PlayerController.RunTo(To, Speed);

            var candidates = Controller.SpawnCandidates(From, Randomization);
            for (var i = 0; i < candidates.Length; i++)
            {
                var c = candidates[i];
                var target = To + new Vector3(
                    (Random.value * 2 - 1) * Randomization,
                    0,
                    (Random.value * 2 - 1) * Randomization);
                c.RunTo(target, Speed).OnCompleted += () => c.Remove();
                SoundController.Play(SplashSoundId.Politician + "-" + (i + 1), c.gameObject);
            }

            SoundController.Play(SplashSoundId.Player, Controller.PlayerController.gameObject);
        }

        public override void Update()
        {
            _t += Time.deltaTime;
            if (_t >= AnimationTime) IsCompleted = true;
        }
    }
}