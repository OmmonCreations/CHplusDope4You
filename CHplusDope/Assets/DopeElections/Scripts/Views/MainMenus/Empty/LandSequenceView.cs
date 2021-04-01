using DopeElections.Progression;
using DopeElections.ScriptedSequences.LandInMainMenu;
using Essentials;
using Progression;
using UnityEngine;

namespace DopeElections.MainMenus.Empty
{
    public class LandSequenceView : MainMenuView
    {
        public override NamespacedKey Id => MainMenuViewId.LandSequence;

        [SerializeField] private LandInMainMenuCinematicController _cinematicController = null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _cinematicController.Initialize();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            _cinematicController.Play(OnSequenceCompleted);
        }

        protected override void OnClose()
        {
            base.OnClose();
            _cinematicController.HotAirBalloon.gameObject.SetActive(false);
        }

        private void OnSequenceCompleted()
        {
            var entry = DopeElectionsApp.Instance.User.UserJourney.GetEntry(UserJourneyStepId.MainMenuLanding);
            entry.State = ProgressEntry.ProgressState.Completed;
            DopeElectionsApp.Instance.User.Save();
            Close();
            Views.OverviewView.Open();
        }
    }
}