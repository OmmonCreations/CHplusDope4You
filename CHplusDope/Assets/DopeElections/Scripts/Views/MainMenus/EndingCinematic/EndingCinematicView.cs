using DopeElections.Progression;
using DopeElections.ScriptedSequences.EndingComic;
using Essentials;
using Progression;
using UnityEngine;

namespace DopeElections.MainMenus.EndingCinematic
{
    public class EndingCinematicView : MainMenuView
    {
        public override NamespacedKey Id => MainMenuViewId.EndingCinematic;

        [SerializeField] private EndingCinematicController _cinematicController = null;

        public EndingCinematicController CinematicController => _cinematicController;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CinematicController.Initialize();
            CinematicController.Environment.SetActive(false);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            CinematicController.Environment.SetActive(true);
            CinematicController.Play(Continue, Abort);
        }

        protected override void OnClose()
        {
            base.OnClose();
            CinematicController.Environment.SetActive(false);
        }

        private void Continue()
        {
            var user = DopeElectionsApp.Instance.User;
            var progressEntry = user.Questionnaire.Progression.GetEntry(RaceProgressStepId.EndingComic);
            if (progressEntry.State != ProgressEntry.ProgressState.Completed)
            {
                progressEntry.State = ProgressEntry.ProgressState.Completed;
                user.Save();
            }

            Views.EndingCreditsView.Open();
        }

        private void Abort()
        {
            if (!Application.isPlaying) return;
            DopeElectionsRouter.GoToProgress();
        }
    }
}