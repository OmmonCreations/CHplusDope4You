using System;
using DopeElections.Answer;
using Essentials;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Candidates
{
    public class CandidateView : GenericView, IView<Candidate>
    {
        public override NamespacedKey Id => GenericViewId.Candidate;

        [SerializeField] private CandidateSubviews _subviews = null;
        [SerializeField] private CandidateDetailsController _detailsController = null;
        [SerializeField] private Button _backButton = null;

        private Action _callback;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            _subviews.Initialize();
            _backButton.onClick.AddListener(Back);
            _detailsController.MainPanelController.HideImmediate();

            var smartSpiderController = _detailsController.SmartSpiderController;
            smartSpiderController.onInfoButtonClick.AddListener(OpenSmartSpiderInfo);
            smartSpiderController.onStatsButtonClick.AddListener(OpenSmartSpiderStats);
        }

        public void Open(Candidate candidate)
        {
            _detailsController.Candidate = candidate;
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            _detailsController.MainPanelController.Show();
        }

        protected override StateChangePromise PrepareClose()
        {
            var result = new StateChangePromise();
            _detailsController.MainPanelController.Hide().Then(result.Fulfill);
            return result;
        }

        private void OpenSmartSpiderInfo()
        {
            Views.SmartSpiderInfoView.Open();
        }

        private void OpenSmartSpiderStats()
        {
            throw new NotImplementedException();
        }

        private void Back()
        {
            Close();
        }
    }
}