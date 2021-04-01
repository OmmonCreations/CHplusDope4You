using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using DopeElections.Questions;
using DopeElections.Users;
using Essentials;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections
{
    public class CandidateAnswersView : DopeElectionsView, IView
    {
        public override NamespacedKey Id => GenericViewId.CandidateAnswers;

        [SerializeField] private QuestionCategoryAnswersController _template = null;
        [SerializeField] private RectTransform _entriesArea = null;
        [SerializeField] private ScrollRect _scrollRect = null;

        [Header("Scene References")] [SerializeField]
        private LocalizedText _tabHeaderText = null;

        private ActiveUser User { get; set; }
        private Questionnaire Questionnaire { get; set; }
        private Candidate _candidate;

        private QuestionCategoryAnswersController[] _categoryEntries = null;

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _tabHeaderText.key = LKey.Views.CandidateAnswers.Title;
            _template.gameObject.SetActive(false);

            DopeElectionsApp.Instance.UserChanged += ApplyUser;
            ApplyUser(DopeElectionsApp.Instance.User);
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            DopeElectionsApp.Instance.UserChanged -= ApplyUser;
            ApplyUser(null);
        }

        public new void Open()
        {
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            UpdateState();
        }

        private void ApplyUser(ActiveUser user)
        {
            if (User != null) User.QuestionnaireChanged -= ApplyQuestionnaire;
            if (!this) return;
            User = user;
            if (user != null)
            {
                user.QuestionnaireChanged += ApplyQuestionnaire;
                ApplyQuestionnaire(user.Questionnaire);
            }
        }

        private void ApplyQuestionnaire(Questionnaire questionnaire)
        {
            Questionnaire = questionnaire;
            UpdateContents();
        }

        private void ApplyCandidate(Candidate candidate)
        {
            if (candidate == _candidate) return;
            _candidate = candidate;
            if (_categoryEntries != null)
            {
                foreach (var e in _categoryEntries) e.Candidate = candidate;
            }

            UpdateState();
        }

        private void UpdateContents()
        {
            ClearContents();
            if (Questionnaire == null) return;
            _entriesArea.gameObject.SetActive(false);
            _categoryEntries = CreateEntries(Questionnaire.Categories);
            _entriesArea.gameObject.SetActive(true);

            if (_categoryEntries.Length > 0)
            {
                _categoryEntries[0].ExpandImmediate();
                _scrollRect.verticalNormalizedPosition = 1;
            }
        }

        private void ClearContents()
        {
            if (_categoryEntries == null) return;
            foreach (var e in _categoryEntries)
            {
                e.Remove();
            }

            _categoryEntries = null;
        }

        private QuestionCategoryAnswersController[] CreateEntries(QuestionCategory[] categories)
        {
            return categories.Select((c, index) => CreateEntry(index, c)).ToArray();
        }

        private QuestionCategoryAnswersController CreateEntry(int index, QuestionCategory category)
        {
            var categoryId = category.id;
            var questionIds = User.Questionnaire.Questions
                .Where(q => q.categoryId == categoryId)
                .Select(q => q.id)
                .ToArray();

            var instanceObject = Instantiate(_template.gameObject, _entriesArea, false);
            var instance = instanceObject.GetComponent<QuestionCategoryAnswersController>();
            instance.Initialize(User, index, categoryId, questionIds);
            instance.Candidate = Candidate;
            instanceObject.SetActive(true);
            return instance;
        }

        private void UpdateState()
        {
            if (_categoryEntries != null)
            {
                foreach (var e in _categoryEntries) e.UpdateState();
            }
        }
    }
}