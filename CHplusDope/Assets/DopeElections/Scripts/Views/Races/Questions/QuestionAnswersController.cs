using System.Linq;
using DopeElections.Answer;
using Localizator;
using UnityEngine;

namespace DopeElections.Races
{
    public abstract class QuestionAnswersController : MonoBehaviour
    {
        public delegate void AnswerEvent(QuestionAnswer answer);

        public event AnswerEvent Answered = delegate { };

        [SerializeField] private string _questionType = null;
        [SerializeField] private QuestionAnswerController _template = null;
        [SerializeField] private RectTransform _answersArea = null;
        [SerializeField] private QuestionAnswerController _undecidedAnswer = null;
        [SerializeField] private float _height = 0;

        private QuestionAnswerController[] _controllers = null;

        private QuestionAnswer[] _answers;
        private QuestionAnswer _answer;

        public string QuestionType => _questionType;
        protected QuestionAnswerController[] Controllers => _controllers;
        protected QuestionAnswerController UndecidedController => _undecidedAnswer;
        public float Height => _height;

        public QuestionAnswer[] Answers
        {
            get => _answers;
        }

        public QuestionAnswer Answer
        {
            get => _answer;
            set => ApplyAnswer(value);
        }

        #region Control

        public void Initialize()
        {
            _template.gameObject.SetActive(false);
            _undecidedAnswer.onSelected.AddListener(active =>
            {
                if (active) Answer = _undecidedAnswer.Answer;
            });
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        protected void OnDestroy()
        {
            OnDestroyed();
        }

        protected virtual void OnDestroyed()
        {
        }

        #endregion

        #region Visibility

        public void ShowImmediate() => ShowImmediate(true);
        public void HideImmediate() => ShowImmediate(false);

        public void ShowImmediate(bool show)
        {
            gameObject.SetActive(show);
            OnShowImmediate(show);
        }

        public void Show() => Show(true);
        public void Hide() => Show(false);

        public void Show(bool show)
        {
            gameObject.SetActive(true);
            OnShow(show);
        }

        #endregion

        #region Public API

        public abstract QuestionAnswer[] GetDefaultAnswers(Question question);
        protected abstract LocalizationKey GetLabel(QuestionAnswer answer);

        public void SetValueWithoutNotify(QuestionAnswer answer)
        {
            _answer = answer;
            if (_controllers != null)
            {
                foreach (var c in _controllers)
                {
                    var selected = c.Answer == answer;
                    if (selected == c.Selected) continue;
                    c.SetSelectedWithoutNotify(selected);
                }
            }

            OnValueChanged(answer);
        }

        public void UpdateAnswers(QuestionAnswer[] answers, QuestionAnswer selected)
        {
            _answers = answers;
            _answer = selected;
            CreateAnswers();
            OnAnswersUpdated();
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnShow(bool show)
        {
        }

        protected virtual void OnShowImmediate(bool show)
        {
        }

        protected virtual void OnValueChanged(QuestionAnswer answer)
        {
        }

        protected virtual void OnAnswersUpdated()
        {
        }

        #endregion

        #region Logic

        private void SelectUndecided()
        {
            Answer = Answers.FirstOrDefault(a => a.answer == -1);
        }

        private void ApplyAnswer(QuestionAnswer answer)
        {
            SetValueWithoutNotify(answer);
            Answered(answer);
        }

        private void CreateAnswers()
        {
            ClearAnswers();
            var undecidedAnswer = Answers.FirstOrDefault(a => a.answer == -1);
            if (undecidedAnswer != null)
            {
                _undecidedAnswer.Answer = undecidedAnswer;
                _undecidedAnswer.Label = GetLabel(undecidedAnswer);
                _undecidedAnswer.SetSelectedWithoutNotify(undecidedAnswer == Answer);
            }

            _undecidedAnswer.gameObject.SetActive(undecidedAnswer != null);
            _controllers = CreateAnswers(Answers, Answer);
        }

        private void ClearAnswers()
        {
            if (_controllers == null) return;
            foreach (var a in _controllers)
            {
                a.Remove();
            }

            _controllers = null;
        }

        private QuestionAnswerController[] CreateAnswers(QuestionAnswer[] answers, QuestionAnswer selected)
        {
            return answers.Where(a => a.answer != -1).Select(a => CreateAnswer(a, selected)).ToArray();
        }

        private QuestionAnswerController CreateAnswer(QuestionAnswer answer, QuestionAnswer selected)
        {
            var instanceObject = Instantiate(_template.gameObject, _answersArea, false);
            instanceObject.name = answer.answer.ToString();

            var instance = instanceObject.GetComponent<QuestionAnswerController>();
            instance.Answer = answer;
            instance.Label = GetLabel(answer);
            instance.Selected = answer == selected;
            instance.onSelected.AddListener(active =>
            {
                if (active) Answer = answer;
            });
            instanceObject.SetActive(true);

            return instance;
        }

        #endregion
    }
}