using System;
using AppManagement;
using DopeElections.Accounts;
using DopeElections.MainMenus;
using DopeElections.Progress;
using DopeElections.Progression;
using DopeElections.RaceCategorySelections;
using DopeElections.RaceResults;
using DopeElections.Races;
using DopeElections.Splash;
using Essentials;

namespace DopeElections
{
    public static class DopeElectionsRouter
    {
        public static void GoToStartup(Action callback)
        {
            ApplicationController.LoadScene(SceneId.Startup, new PostSceneLoadAction(callback));
        }

        public static void GoToSplash(bool startupComplete = true)
        {
            ApplicationController.LoadScene(SceneId.Splash, new SplashSceneLoadParams(startupComplete));
        }

        public static void GoToAccount(NamespacedKey viewId = default)
        {
            ApplicationController.LoadScene(SceneId.Account, new AccountSceneLoadParams(viewId));
        }

        public static void GoToAccountCreation(Action<bool> callback)
        {
            var user = DopeElectionsApp.Instance.User;
            var loadParams = new AccountSceneLoadParams(AccountViewId.FaceSelection, callback);
            ApplicationController.LoadScene(SceneId.Account, loadParams);
        }

        public static void GoToMainMenu(NamespacedKey viewId = default)
        {
            ApplicationController.LoadScene(SceneId.MainMenu, new MainMenuSceneLoadParams(viewId));
        }

        public static void GoToProgress()
        {
            ApplicationController.LoadScene(SceneId.Progress);
        }

        public static void GoToProgress(IRaceProgressEntry focused)
        {
            ApplicationController.LoadScene(SceneId.Progress,
                new ProgressSceneLoadParams(ProgressViewId.Progress, focused));
        }

        public static void GoToLeaderboard()
        {
            ApplicationController.LoadScene(SceneId.Progress,
                new ProgressSceneLoadParams(ProgressViewId.Leaderboard, null));
        }

        public static void GoToRaceCategorySelection(RaceContext context)
        {
            var loadParams = new RaceCategorySelectionSceneLoadParams(context);
            ApplicationController.LoadScene(SceneId.RaceCategorySelection, loadParams);
        }

        public static void GoToRace(RaceContext context)
        {
            var loadParams = new RaceSceneLoadParams(context);
            ApplicationController.LoadScene(SceneId.Race, loadParams);
        }

        public static void GoToRaceResult(RaceContext context, bool playRaceResultSequence)
        {
            var loadParams = new RaceResultSceneLoadParams(context, playRaceResultSequence);
            ApplicationController.LoadScene(SceneId.RaceResult, loadParams);
        }
    }
}