using System.Linq;
using MobileInputs;
using UnityEngine.InputSystem;

namespace DopeElections.Races
{
    public class RunQuestionRaceState : QuestionRaceState
    {
        private RaceCandidateController[] Candidates { get; }

        public RunQuestionRaceState(QuestionMarathonRaceController raceController, QuestionRace race) : base(raceController,
            race)
        {
            Candidates = raceController.CandidateControllers;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            RaceController.OverlayController.Hide();

            var raceController = RaceController;
            var interactionSystem = raceController.InteractionSystem;
            var groupController = raceController.GroupController;

            raceController.CameraController.FollowGroup();
            
            interactionSystem.OnPointerDown += OnPointerDown;

            var questions = Race.Marathon.Questions.Take(Race.Index + 1).ToArray();
            var reactState = groupController.ReactToAnswer(questions, Race.Answer);
            reactState.OnCompleted += () => IsCompleted = true;
        }

        private void OnPointerDown(IInteractable interactable, InputAction.CallbackContext context)
        {
            Skip();
        }

        public override void Update()
        {
            // wait for group reaction to end
        }

        private void Skip()
        {
            IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            RaceController.GroupController.Stroll();
            RaceController.AnnounceFinishQuestion(Race);
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            RaceController.InteractionSystem.OnPointerDown -= OnPointerDown;
        }
    }
}