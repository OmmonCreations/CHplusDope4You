using System.Linq;
using AppManagement;
using DopeElections.Candidates;
using DopeElections.Elections;
using DopeElections.Users;
using Essentials;
using UnityEngine;
using UnityEngine.Serialization;

namespace DopeElections.RaceCategorySelections
{
    public class RaceCategorySelectionSceneController : SceneController
    {
        public override NamespacedKey Id => SceneId.RaceCategorySelection;
        public override InitializeTrigger Initialization => InitializeTrigger.AfterLaunch;
        
        [FormerlySerializedAs("_viewsController")] [SerializeField]
        private RaceCategorySelectionViewsContainer _viewsContainer = null;

        [SerializeField] private CandidateController[] _candidates = null;

        private RaceCategorySelectionViewsContainer Views => _viewsContainer;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Views.Initialize();
        }

        protected override void OnLoad()
        {
            var loadParams = GetSceneLoadParams<RaceCategorySelectionSceneLoadParams>();
            var context = loadParams != null ? loadParams.Context : null;
            if (context == null)
            {
                Debug.LogError("No race context provided.");
                DopeElectionsRouter.GoToProgress();
                return;
            }

            AssignRandomCandidates(_candidates);

            Views.BlackMask.BlockInteractions(false);
            Views.BlackMask.FadeToClear();

            Views.RandomSelectionView.Open(loadParams.Context);
        }

        private void AssignRandomCandidates(CandidateController[] candidates)
        {
            var elections = DopeElectionsApp.Instance.User.GetAvailableElections();
            var availableCandidates = elections.SelectMany(e => e.GetCandidates()).DistinctBy(c => c.id)
                .OrderBy(c => Random.value).Take(candidates.Length).ToList();
            for (var i = 0; i < availableCandidates.Count; i++)
            {
                candidates[i].Candidate = availableCandidates[i];
            }

            for (var i = availableCandidates.Count; i < candidates.Length; i++)
            {
                candidates[i].gameObject.SetActive(false);
            }
        }
    }
}