using DopeElections.Localizations;
using SpeechBubbles;

namespace DopeElections.ScriptedSequences.LandInMainMenu.States
{
    public class ShowSpeechBubbleState : LandInMainMenuSequenceState
    {
        public const float ReadTime = 3;
    
        public ShowSpeechBubbleState(LandInMainMenuCinematicController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var localization = Controller.Localization;
            var text = localization.GetString(LKey.ScriptedSequences.LandInMainMenu.WithMyVeryOwnRaceRoyale);
            var speechBubble =
                Controller.SpeechBubbles.ShowSpeechBubble(new TextSpeechBubble(Controller.Player.SpeechBubbleAnchor, text));
            speechBubble.HideAfter(ReadTime).Then(() => IsCompleted = true);
        }

        public override void Update()
        {
        }
    }
}