using System.Linq;
using AppManagement;
using DopeElections.Parliaments;
using DopeElections.Progression;
using DopeElections.Sounds;
using DopeElections.Users;
using Essentials;
using FMODSoundInterface;
using Progression;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Views;

namespace DopeElections.Progress
{
    public class ProgressSceneController : SceneController
    {
        public override NamespacedKey Id => SceneId.Progress;
        public override InitializeTrigger Initialization => InitializeTrigger.AfterLaunch;

        [FormerlySerializedAs("_viewsController")] [SerializeField]
        private ProgressViewsContainer _viewsContainer = null;

        [Header("Scene References")] [SerializeField]
        private PlanetProgressController _progressionController = null;

        [SerializeField] private ParliamentController _parliament = null;

        private ProgressViewsContainer Views => _viewsContainer;
        private PlanetProgressController ProgressionController => _progressionController;

        private ActiveUser User { get; set; }
        private RaceProgressionTree Tree { get; set; }
        private IRaceProgressEntry[] RaceProgressEntries { get; set; }
        private IRaceProgressEntry ActiveEntry { get; set; }
        private int ActiveEntryIndex { get; set; }

        private bool EndingComicSeen { get; set; }
        private bool AllRacesCompleted { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Views.Initialize();
        }

        protected override void OnLoad()
        {
            var loadParams = GetSceneLoadParams<ProgressSceneLoadParams>();

            var user = DopeElectionsApp.Instance.User;
            var questionnaire = user.Questionnaire;
            var tree = questionnaire != null ? questionnaire.Progression : null;
            if (questionnaire == null || tree == null)
            {
                Debug.LogWarning("No questionnaire loaded.");
                DopeElectionsRouter.GoToMainMenu(SceneId.MainMenu);
                return;
            }

            User = user;
            Tree = tree;

#if UNITY_EDITOR
            // cheat: instantly finish all steps
            if (Keyboard.current.fKey.isPressed && Keyboard.current.shiftKey.isPressed)
            {
                CompleteAllSteps();
            }
#endif

            LoadReferences(loadParams);

            ApplyProgressionTree(tree);

            Views.LeaderboardView.UpdateLeaderboard();
            Views.BlackMask.Alpha = 1;
            MusicController.Play(Music.MainMenu);

            ProgressionController.FocusImmediate(ActiveEntry);
            ProgressionController.HideLabelsImmediate();

            _parliament.Locked = !EndingComicSeen;

            // open requested view
            OpenFirstView(loadParams);
        }

        private void LoadReferences(ProgressSceneLoadParams loadParams)
        {
            var questionnaire = User.Questionnaire;

            var raceProgressEntries = Tree.Entries.OfType<IRaceProgressEntry>().ToArray();
            RaceProgressEntries = raceProgressEntries;
            ActiveEntry = GetFocusedEntry(loadParams);
            ActiveEntryIndex = ActiveEntry != null ? raceProgressEntries.IndexOf(ActiveEntry) : 0;

            EndingComicSeen = questionnaire.Progression.GetEntry(RaceProgressStepId.EndingComic).State ==
                              ProgressEntry.ProgressState.Completed;
            AllRacesCompleted = raceProgressEntries
                .All(e => e.State == ProgressEntry.ProgressState.Completed);
        }

        private IRaceProgressEntry GetFocusedEntry(ProgressSceneLoadParams loadParams)
        {
            var result = loadParams != null && loadParams.FocusedProgressEntry != null
                ? loadParams.FocusedProgressEntry
                : RaceProgressEntries
                    .Where(e => e.State != ProgressEntry.ProgressState.Completed)
                    .DefaultIfEmpty(RaceProgressEntries.Last()).First();
            return result;
        }

        private void OpenFirstView(ProgressSceneLoadParams loadParams)
        {
            if (loadParams != null && loadParams.ViewId != default && loadParams.ViewId != ProgressViewId.Progress)
            {
                var view = Views.GetView<IView>(loadParams.ViewId);
                if (view == null)
                {
                    Debug.LogError("View " + loadParams.ViewId + " not found!");
                    DopeElectionsRouter.GoToMainMenu();
                    return;
                }

                view.Open();
                return;
            }

            var extraInfo = Tree.ExtraInfoPositions.TryGetValue(ActiveEntryIndex, out var extra) ? extra : null;
            if (extraInfo != null && extraInfo.State != ProgressEntry.ProgressState.Completed)
            {
                var step = ProgressionController.Controllers[ActiveEntryIndex];
                var talker = step.ExtraInfoTalker;
                Views.ExtraInfoView.Open(extraInfo, talker);
                return;
            }

            if (AllRacesCompleted && !EndingComicSeen)
            {
                Views.CongratulationsView.Open();
                return;
            }

#if UNITY_EDITOR
            // cheat: instantly view congratulations
            if (Keyboard.current.fKey.isPressed && !Keyboard.current.shiftKey.isPressed)
            {
                Views.CongratulationsView.Open();
                return;
            }
#endif

            Views.ProgressionView.Open(ActiveEntry);
        }

        private void CompleteAllSteps()
        {
            var raceEntries = Tree.Entries.OfType<IRaceProgressEntry>().ToList();
            foreach (var e in raceEntries.Take(raceEntries.Count - 1)
                .Where(e => e.State != ProgressEntry.ProgressState.Completed))
            {
                e.State = ProgressEntry.ProgressState.Completed;
            }

            User.Save();
        }

        private void ApplyProgressionTree(ProgressionTree tree)
        {
            // try to unlock the next step
            var visibleEntries = tree.Entries.OfType<IVisibleProgressEntry>().ToList();
            foreach (var e in visibleEntries) e.UpdateLabel();

            ProgressionController.Tree = tree;
            ProgressionController.PlacePlayer(visibleEntries.LastOrDefault(e => e.IsAvailable));
        }
    }
}