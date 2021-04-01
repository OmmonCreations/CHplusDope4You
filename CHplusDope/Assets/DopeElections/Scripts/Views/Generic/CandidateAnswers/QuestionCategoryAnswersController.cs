using System.Linq;
using AnimatedObjects;
using DopeElections.Answer;
using DopeElections.AnswerLabelTypes;
using DopeElections.Localizations;
using DopeElections.Users;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections
{
    public class QuestionCategoryAnswersController : MonoBehaviour
    {
        [SerializeField] private LocalizedText _titleText = null;
        [SerializeField] private LocalizedText _matchAmountText = null;
        [SerializeField] private LocalizedText _progressText = null;
        [SerializeField] private LocalizedText _matchLabelText = null;
        [SerializeField] private QuestionAnswerDisplayController _template = null;
        [SerializeField] private RectTransform _entriesArea = null;
        [SerializeField] private PickAnswerLabelsController _standard4ColumnLabels = null;
        [SerializeField] private SliderAnswerLabelsController _slider7ColumnLabels = null;
        [SerializeField] private Button _toggleExpandButton = null;
        [SerializeField] private ToggleableObjectController _expandController = null;

        private ActiveUser _user;
        private Candidate _candidate;

        private int _categoryId;

        private QuestionAnswerDisplayController[] _entries;

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        public void Initialize(ActiveUser user, int index, int categoryId, int[] questionIds)
        {
            _user = user;

            _categoryId = categoryId;

            _template.gameObject.SetActive(false);

            _standard4ColumnLabels.Initialize();
            _slider7ColumnLabels.Initialize();

            _titleText.key = LKey.Views.CandidateAnswers.CategoryTitle;
            _progressText.key = LKey.Views.CandidateAnswers.CategoryProgressValue;
            _matchAmountText.key = LKey.Views.CandidateAnswers.CategoryMatchValue;
            _matchLabelText.key = LKey.Views.CandidateAnswers.CategoryMatchLabel;
            _titleText.SetVariable("index", (index + 1).ToString());

            _entries = CreateEntries(questionIds);

            _toggleExpandButton.onClick.AddListener(() => _expandController.Show(!_expandController.IsVisible));
            _expandController.HideImmediate();

            UpdateLabels();
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        public void ExpandImmediate() => ExpandImmediate(true);

        public void CollapseImmediate() => ExpandImmediate(false);

        public void ExpandImmediate(bool expand)
        {
            _expandController.ShowImmediate(expand);
        }

        private void ApplyCandidate(Candidate candidate)
        {
            _candidate = candidate;
            if (_entries != null)
            {
                foreach (var e in _entries) e.Candidate = candidate;
            }

            UpdateState();
        }

        private QuestionAnswerDisplayController[] CreateEntries(int[] questionIds)
        {
            return questionIds.Select((questionId, index) => CreateEntry(index, questionId)).ToArray();
        }

        private QuestionAnswerDisplayController CreateEntry(int index, int questionId)
        {
            var instanceObject = Instantiate(_template.gameObject, _entriesArea, false);
            var instance = instanceObject.GetComponent<QuestionAnswerDisplayController>();
            instance.Initialize(_user, index, questionId);
            instanceObject.SetActive(true);
            return instance;
        }

        public void UpdateState()
        {
            var questionnaire = _user.Questionnaire;
            if (questionnaire == null) return;

            var candidate = Candidate;
            var categoryId = _categoryId;
            var questions = questionnaire.Questions.Where(q => q.categoryId == categoryId).ToList();

            if (candidate != null)
            {
                var userAnswers = _user.Questionnaire.Progression.UserAnswers
                    .Where(a => questions.Any(q => q.id == a.questionId));
                var categoryMatch = candidate.CalculateMatch(userAnswers);
                _matchAmountText.SetVariable("match", Mathf.RoundToInt(categoryMatch * 100).ToString());
            }

            var answeredCount = questions.Count(
                q => questionnaire.Progression.UserAnswers.Any(a => a.questionId == q.id)
            );

            _progressText.SetVariable("answered", answeredCount.ToString());
            _progressText.SetVariable("questions", questions.Count.ToString());

            if (_entries != null)
            {
                foreach (var e in _entries) e.UpdateState();
            }
        }

        public void UpdateLabels()
        {
            var user = _user;
            var questionnaire = user.Questionnaire;
            if (questionnaire == null) return;
            var category = questionnaire.Categories.FirstOrDefault(c => c.id == _categoryId);
            if (category != null)
            {
                _titleText.SetVariable("category", category.name);

                var firstQuestion = questionnaire.Questions.FirstOrDefault(q => q.categoryId == category.id);
                if (firstQuestion != null)
                {
                    _standard4ColumnLabels.gameObject.SetActive(firstQuestion.type == "Standard-4");
                    _slider7ColumnLabels.gameObject.SetActive(firstQuestion.type == "Slider-7");
                }
            }


            if (_entries != null)
            {
                foreach (var e in _entries) e.UpdateLabels();
            }
        }
    }
}