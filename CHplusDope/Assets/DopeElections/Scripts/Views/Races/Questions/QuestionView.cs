using System;
using System.Linq;
using AnimatedObjects;
using DopeElections.Answer;
using DopeElections.Progression;
using Essentials;
using FMODSoundInterface;
using Localizator;
using Progression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Views;
using Views.UI;

namespace DopeElections.Races
{
    public class QuestionView : RaceView, IView<Question, Action<int>>
    {
        public override NamespacedKey Id => RaceViewId.Question;

        [Header("References")] [SerializeField]
        private RectTransform _frameTransform = null;

        [SerializeField] private RectTransform _questionTextArea = null;
        [SerializeField] private LocalizedText _questionText = null;
        [SerializeField] private TMP_Text _questionTextComponent = null;
        [SerializeField] private Button _questionInfoButton = null;
        [SerializeField] private Toggle _muteRaceMusicToggle = null;
        [SerializeField] private SlidablePanelController _panelController = null;
        [SerializeField] private QuestionAnswersController[] _answerControllers = null;
        [Header("Layout")] [SerializeField] private DeviceSafeArea _safeArea = null;
        [SerializeField] private LayoutElement[] _answerAreas = null;
        [SerializeField] private float _maxHeight = 500;

        [Tooltip("Height of the frame without the height of the text viewport and answer area")] [SerializeField]
        private float _extraHeight = 400;

        private Question Question { get; set; }
        private Action<int> Callback { get; set; }
        private QuestionAnswer _selected = null;

        private SlidablePanelController PanelController => _panelController;

        #region View Control

        protected override void OnInitialize()
        {
            base.OnInitialize();
            foreach (var answerController in _answerControllers)
            {
                answerController.Initialize();
                answerController.Answered += OnAnswered;
                answerController.HideImmediate();
            }

            _questionInfoButton.onClick.AddListener(OpenQuestionInfo);
            _muteRaceMusicToggle.onValueChanged.AddListener(OnMuteRaceMusicChanged);
        }

        public void Open(Question question, Action<int> callback)
        {
            Question = question;
            Callback = callback;
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            var question = Question;

            var user = DopeElectionsApp.Instance.User;
            var userAnswer = user.Questionnaire.Progression.UserAnswers
                .FirstOrDefault(a => a.questionId == question.id);

            var questionType = question.type;
            var answerController = _answerControllers.FirstOrDefault(c => c.QuestionType == questionType);
            foreach (var controller in _answerControllers.Where(c => c != answerController)) controller.HideImmediate();
            if (!answerController)
            {
                Debug.LogError($"No answer controller for question type {questionType} found!");
                return;
            }

            _questionText.key = new LocalizationKey {fallback = question.text};
            _questionText.textComponent.alignment = question.text.Length > 128
                ? TextAlignmentOptions.Justified
                : TextAlignmentOptions.Left;

            gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_frameTransform);
            var textareaSize = _questionTextArea.rect.size;

            var textSize = _questionTextComponent.GetPreferredValues(
                question.text,
                textareaSize.x,
                textareaSize.y
            );

            var frameHeight = Mathf.Min(
                textSize.y + _extraHeight + _safeArea.RectTransform.offsetMin.y + answerController.Height,
                _maxHeight
            );

            _frameTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, frameHeight);
            foreach (var a in _answerAreas)
            {
                a.preferredHeight = answerController.Height;
            }

            var answers = answerController.GetDefaultAnswers(question);
            var answer = userAnswer != null ? answers.FirstOrDefault(a => a.answer == userAnswer.answer) : null;
            answerController.UpdateAnswers(answers, answer);

            answerController.HideImmediate();
            PanelController.DisappearDelta = new Vector2(0, -(frameHeight + 50));
            PanelController.Show().Then(answerController.Show);

            _selected = answer;

            var muteRaceMusic = DopeElectionsApp.Instance.Settings.GetValue(Setting.EnableRaceMusic);
            _muteRaceMusicToggle.SetIsOnWithoutNotify(muteRaceMusic);
            _questionInfoButton.interactable = !string.IsNullOrWhiteSpace(question.info != null 
                ? question.info.Replace("<p><br></p>", "") 
                : null
            );
            
            Views.BlackMask.BlockInteractions(false);

            MusicController.Duck(true, 1f);

            if (user.UserJourney.GetEntry(UserJourneyStepId.RaceInfo).State != ProgressEntry.ProgressState.Completed)
            {
                DopeElectionsApp.Instance.Views.RaceInfoView.Open();
                DopeElectionsApp.Instance.Views.RaceInfoView.Closed += OnRaceInfoClosed;
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
            MusicController.Duck(false);
        }

        #endregion
        
        #region Navigation

        private void OpenQuestionInfo()
        {
            Views.QuestionInfoView.Open(Question);
        }
        
        #endregion

        #region Event Listeners

        private void OnRaceInfoClosed()
        {
            DopeElectionsApp.Instance.Views.RaceInfoView.Closed -= OnRaceInfoClosed;
            var user = DopeElectionsApp.Instance.User;
            var entry = user.UserJourney.GetEntry(UserJourneyStepId.RaceInfo);
            entry.State = ProgressEntry.ProgressState.Completed;
            user.Save();
        }

        private void OnAnswered(QuestionAnswer answer)
        {
            _selected = answer;
            Confirm();
        }

        private void OnMuteRaceMusicChanged(bool value)
        {
            var settings = DopeElectionsApp.Instance.Settings;
            settings.SetValue(Setting.EnableRaceMusic, value);
            DopeElectionsApp.Instance.SaveSettings();
        }

        #endregion

        #region Logic

        private void Confirm()
        {
            if (_selected == null) return;
            PanelController.Hide().Then(() => Callback(_selected.answer));
        }

        #endregion
    }
}