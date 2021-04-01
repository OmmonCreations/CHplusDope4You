using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using DopeElections.Progression;
using DopeElections.Races;
using Essentials;
using Localizator;
using Progression;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DopeElections.RaceCategorySelections
{
    public class RandomSelectionView : RaceCategorySelectionView
    {
        public override NamespacedKey Id => RaceViewId.CategorySelection;

        [SerializeField] private CategoriesContainer _container = null;
        [SerializeField] private Button _rerollButton = null;
        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private Button _backButton = null;

        [SerializeField] private LocalizedText _titleText = null;
        [SerializeField] private LocalizedText _overviewText = null;
        [SerializeField] private LocalizedText _instructionText = null;
        [SerializeField] private LocalizedText _confirmText = null;

        private RaceCategoryProgressEntry CategoryEntry { get; set; }

        private QuestionCategory Selected
        {
            get => _container.Selected;
            set => _container.Selected = value;
        }

        #region Initializer

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _backButton.onClick.AddListener(Back);
            _rerollButton.onClick.AddListener(Reroll);
            _confirmButton.onClick.AddListener(Confirm);
            _confirmButton.gameObject.SetActive(false);
            _container.CategorySelected += SelectCategory;

            _titleText.key = LKey.Views.RandomCategorySelection.Title;
            _overviewText.key = LKey.Views.RandomCategorySelection.ChallengeOverview;
            _confirmText.key = LKey.Views.RandomCategorySelection.Confirm;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            CategoryEntry = Context.ProgressEntry as RaceCategoryProgressEntry;
            _container.SetSelectedImmediate(CategoryEntry != null ? CategoryEntry.GetCategory() : null);
            CreateEntries();
            UpdateState();
        }

        #endregion

        #region Navigation

        private void Confirm()
        {
            var category = Selected;
            if (category == null) return;
            var previous = CategoryEntry.Configuration;
            if (category.id != previous)
            {
                CategoryEntry.Configuration = category.id;
                DopeElectionsApp.Instance.User.Save();
            }

            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() => { DopeElectionsRouter.GoToRace(Context); });
        }

        private void Back()
        {
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() => { DopeElectionsRouter.GoToProgress(CategoryEntry); });
        }

        #endregion

        #region Logic

        private void Reroll()
        {
            Selected = null;
            CreateEntries();
            UpdateState();
        }

        private void SelectCategory(QuestionCategory category)
        {
            Selected = category;
            UpdateState();
        }

        private void UpdateState()
        {
            var category = Selected;
            var categorySelected = category != null;
            if (categorySelected != _confirmButton.gameObject.activeSelf)
            {
                _confirmButton.gameObject.SetActive(categorySelected);
            }

            _instructionText.key = categorySelected
                ? LKey.Views.RandomCategorySelection.InstructionConfirm
                : LKey.Views.RandomCategorySelection.InstructionChoose;
            if (category != null)
            {
                var questionnaire = DopeElectionsApp.Instance.User.Questionnaire;
                var allQuestions = questionnaire.Questions;
                var identifier = category.id;
                var questions = allQuestions != null
                    ? allQuestions.Where(q => q.categoryId == identifier).ToArray()
                    : null;

                var userAnswers = DopeElectionsApp.Instance.User.Questionnaire.Progression.UserAnswers;
                var answeredCount = questions != null
                    ? questions.Count(q => userAnswers.Any(a => a.questionId == q.id))
                    : 0;
                _instructionText.SetVariable("questions", (questions != null ? questions.Length : 0).ToString());
                _instructionText.SetVariable("answered", answeredCount.ToString());
            }
        }

        #endregion

        #region Entry Creation

        private void CreateEntries()
        {
            var user = DopeElectionsApp.Instance.User;
            var entry = CategoryEntry;
            var questionnaire = user.Questionnaire;
            var tree = questionnaire.Progression;
            var allCategories = questionnaire.Categories;

            QuestionCategory[] categories;
            if (entry.State == ProgressEntry.ProgressState.Completed && entry.Configuration != 0)
            {
                var category = allCategories.FirstOrDefault(c => c.id == entry.Configuration);
                if (category == null)
                {
                    Debug.LogError("Category is null!");
                    return;
                }

                categories = new[] {category};
                _rerollButton.gameObject.SetActive(false);
            }
            else
            {
                var clearedCategories = tree.Entries
                    .OfType<RaceCategoryProgressEntry>()
                    .Where(e => e.Configuration != 0)
                    .Select(e => allCategories.FirstOrDefault(c => c.id == e.Configuration))
                    .Where(c => c != null).ToList();
                var availableCategories = allCategories.Except(clearedCategories).ToList();
                var availableCount = availableCategories.Count;
                categories = availableCategories.OrderBy(c => Random.value).Take(4).ToArray();

                _rerollButton.gameObject.SetActive(categories.Length < availableCount);
            }

            _container.Categories = categories;
        }

        #endregion
    }
}