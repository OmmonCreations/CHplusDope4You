using UnityEngine;

namespace DopeElections.ScriptedSequences.JourneyToPlanet
{
    public class PrepareCinematicState : JourneyToPlanetCinematicState
    {
        public PrepareCinematicState(JourneyToPlanetCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            Player.transform.SetParent(Balloon.CharacterAnchor, false);
            Player.transform.localPosition = Vector3.zero;
            Player.transform.localRotation = Quaternion.identity;
            Player.transform.localScale = Vector3.one;

            var balloonTransform = Balloon.transform;
            balloonTransform.SetParent(Controller.EnvironmentAnchor, false);
            balloonTransform.localPosition = Vector3.zero;
            balloonTransform.localRotation = Quaternion.identity;
            balloonTransform.localScale = Vector3.one;

            var planetTransform = Planet.transform;
            planetTransform.SetParent(Controller.PlanetAnchor, false);
            planetTransform.localPosition = Vector3.zero;
            planetTransform.localRotation = Quaternion.identity;
            planetTransform.localScale = Vector3.one;

            Planet.HideImmediate();
            Planet.CloudLayer.HideImmediate();

            Balloon.gameObject.SetActive(true);

            Controller.BlackMask.FadeToClear();
            Controller.BlackMask.BlockInteractions(false);
        }

        public override void Update()
        {
            IsCompleted = true;
        }
    }
}