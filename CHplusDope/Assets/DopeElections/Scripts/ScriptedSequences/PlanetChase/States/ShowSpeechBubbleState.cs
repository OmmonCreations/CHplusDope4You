using DopeElections.Localizations;
using SpeechBubbles;

namespace DopeElections.ScriptedSequences.PlanetChase
{
    public class ShowSpeechBubbleState : PlanetChaseCinematicControllerState
    {
        public const float ReadTime = 4;

        public ShowSpeechBubbleState(PlanetChaseCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var localization = Controller.Localization;
            var text = localization.GetString(LKey.ScriptedSequences.PlanetChase.PutCharacterToTheTest);
            var speechBubble =
                Controller.SpeechBubbleLayer.ShowSpeechBubble(new TextSpeechBubble(Controller.Player.SpeechBubbleAnchor, text));
            speechBubble.HideAfter(ReadTime).Then(() => IsCompleted = true);
        }

        public override void Update()
        {
        }
    }
}