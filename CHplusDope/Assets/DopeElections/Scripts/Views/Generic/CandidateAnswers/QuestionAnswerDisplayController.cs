using System.Linq;
using DopeElections.Answer;
using DopeElections.Candidates;
using DopeElections.Localizations;
using DopeElections.Users;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections
{
    public class QuestionAnswerDisplayController : MonoBehaviour
    {
        [SerializeField] private LocalizedText _questionText = null;
        [SerializeField] private LocalizedText _commentText = null;
        [SerializeField] private PickAnswerDisplayController _standard4Answers = null;
        [SerializeField] private SliderAnswerDisplayController _slider7Answers = null;
        [SerializeField] private Button _commentButton = null;
        [SerializeField] private Graphic[] _accentGraphics = null;

        private ActiveUser _user;
        private Candidate _candidate;

        private int _index;
        private int _questionId;

        private AnswerDisplayController _answerDisplay = null;

        private Color _accentColor;

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        public Color AccentColor
        {
            get => _accentColor;
            set => ApplyAccentColor(value);
        }

        public void Initialize(ActiveUser user, int index, int questionId)
        {
            _user = user;

            _index = index;
            _questionId = questionId;

            _questionText.key = LKey.Views.CandidateAnswers.Question;
            _questionText.SetVariable("index", (index + 1).ToString());

            _commentButton.onClick.AddListener(ToggleComment);

            _standard4Answers.gameObject.SetActive(false);
            _slider7Answers.gameObject.SetActive(false);

            var question = user.Questionnaire.Questions.FirstOrDefault(q => q.id == questionId);
            _answerDisplay = question != null && question.type == "Slider-7"
                ? (AnswerDisplayController) _slider7Answers
                : _standard4Answers;
            _answerDisplay.gameObject.SetActive(true);
        }

        private void ApplyAccentColor(Color color)
        {
            _accentColor = color;
            foreach (var g in _accentGraphics) g.color = color;
        }

        private void ApplyCandidate(Candidate candidate)
        {
            _candidate = candidate;
            _commentText.gameObject.SetActive(false);
            AccentColor = candidate != null ? candidate.GetPartyColor() : Color.gray;
            _answerDisplay.AccentColor = AccentColor;
            UpdateState();
        }

        private void ToggleComment()
        {
            _commentText.gameObject.SetActive(!_commentText.gameObject.activeSelf);
        }

        public void UpdateState()
        {
            var candidateAnswer = _candidate != null
                ? _candidate.responses.FirstOrDefault(r => r.questionId == _questionId)
                : null;
            var comment = candidateAnswer != null && candidateAnswer.comment != null ? candidateAnswer.comment : "";
            _commentText.key = new LocalizationKey {fallback = comment};
            _answerDisplay.CandidateAnswer = candidateAnswer;

            var userAnswer = _user.Questionnaire != null
                ? _user.Questionnaire.Progression.UserAnswers.FirstOrDefault(a => a.questionId == _questionId)
                : null;
            _answerDisplay.UserAnswer = userAnswer;

            var hasComment = candidateAnswer != null && !string.IsNullOrWhiteSpace(candidateAnswer.comment);
            _commentButton.gameObject.SetActive(hasComment);

            _answerDisplay.UpdateState();
        }

        public void UpdateLabels()
        {
            var question = _user.Questionnaire != null
                ? _user.Questionnaire.Questions.FirstOrDefault(q => q.id == _questionId)
                : null;
            if (question != null) _questionText.SetVariable("question", question.text);
        }
    }
}