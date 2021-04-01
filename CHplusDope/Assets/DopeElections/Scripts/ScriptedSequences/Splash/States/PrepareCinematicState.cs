using DopeElections.Sounds;
using DopeElections.Users;
using FMODSoundInterface;
using UnityEngine;

namespace DopeElections.ScriptedSequences.Splash
{
    public class PrepareCinematicState : SplashCinematicState
    {
        private const float AnimationTime = 0.2f;

        private float _t;

        public PrepareCinematicState(SplashCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.PlayerController.ApplyUserConfiguration();
            Controller.PlayerController.Face = PlayerFaceId.Frightened;
            Controller.LogoGroup.alpha = 0;
            Controller.SubtitleGroup.alpha = 0;
            Controller.PlayButtonGroup.alpha = 0;
            Controller.BlackMask.Alpha = 1;
            MusicController.Play(Music.Splash);
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.BlackMask.FadeToClear(1);
        }
    }
}