using System;
using DopeElections.Progression;
using DopeElections.ScriptedSequences.LandOnPlanet;
using Essentials;
using Progression;
using UnityEngine;
using Views;

namespace DopeElections.Accounts
{
    public class ComicSequenceView : AccountView, IView
    {
        public override NamespacedKey Id => AccountViewId.ComicSequence;

        [Header("Scene References")] [SerializeField]
        private LandOnPlanetCinematicController _landOnPlanetController = null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _landOnPlanetController.Initialize();
        }

        public new void Open()
        {
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            _landOnPlanetController.Play(OnCinematicCompleted, () => DopeElectionsRouter.GoToSplash());
        }

        private void OnCinematicCompleted()
        {
            var user = DopeElectionsApp.Instance.User;
            var openingComicProgress = user.UserJourney.GetEntry(UserJourneyStepId.OpeningComic);
            openingComicProgress.State = ProgressEntry.ProgressState.Completed;
            user.Save();

            EditorController.Quit();
        }
    }
}