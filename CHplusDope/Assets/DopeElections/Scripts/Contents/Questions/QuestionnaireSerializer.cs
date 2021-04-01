using System;
using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Elections;
using DopeElections.Progression;
using Essentials;
using Newtonsoft.Json.Linq;
using Progression;
using Proyecto26;
using UnityEngine;

namespace DopeElections.Questions
{
    public static class QuestionnaireSerializer
    {
        public static void Save(Questionnaire questionnaire)
        {
            var data = new JObject
            {
                ["answers"] =
                    new JArray(
                        questionnaire.Progression.UserAnswers.Select(a => a.Serialize()).Cast<object>().ToArray()),
                ["category_winners"] =
                    new JArray(
                        questionnaire.Progression.QuestionCategoryWinners.Select(a => a.Serialize()).Cast<object>()
                            .ToArray()),
                ["progression"] = questionnaire.Progression.Save().Serialize()
            };
            var file = GetUserFile(questionnaire.CantonId, questionnaire.ElectionIds);
            var localStorage = DopeElectionsApp.Instance.LocalStorage;
            localStorage.WriteAllText(file, data.ToString());
        }

        public static Questionnaire Load(Canton canton, Constituency constituency, Election election)
        {
            if (canton == null)
            {
                Debug.LogError("Canton is null. Cannot load questionnaire.");
                return null;
            }

            var assets = DopeElectionsApp.Instance.Assets;
            if (election == null)
            {
                var localElections = assets.GetAssets<Election>(e => e.constituencyId == constituency.id);
                var timestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).Seconds;
                var upcomingElections = localElections.Where(e => e.electionTimestamp > timestamp).ToList();
                election = upcomingElections.Count > 0
                    ? upcomingElections.OrderBy(e => e.electionTimestamp).First() // select next upcoming election
                    : localElections.OrderByDescending(e => e.electionTimestamp)
                        .FirstOrDefault(); // select latest past election
                if (election == null) return null;
            }

            var parallelElections = election.GetParallelElections(canton, constituency).ToList();

            var electionIds = parallelElections.Select(e => e.id).ToList();
            var languageSetting = DopeElectionsApp.Instance.Settings.GetValue(Setting.Language);
            var languageCode = BackendCHplusDope.GetLanguageCode(languageSetting);

            var userData = LoadUserData(canton.id, electionIds);
            var questions = LoadQuestions(languageCode, election.id);
            var categories = LoadQuestionCategories(languageCode, election.id, questions);
            var questionAxis = LoadQuestionAxis(languageCode, election.id);

            foreach (var question in questions)
            {
                var axis = questionAxis.Where(a => a.questionId == question.id);
                question.axis = axis.ToArray();
            }

            var tree = RaceProgressionTree.Get(categories.Length);
            if (userData.ProgressionData != null) tree.Load(userData.ProgressionData);
            tree.ApplyAnswers(userData.Answers);
            tree.ApplyQuestionCategoryWinners(userData.CategoryWinners);
            return new Questionnaire(canton.id, electionIds, categories, questions, tree);
        }

        private static UserData LoadUserData(int cantonId, IReadOnlyCollection<int> electionIds)
        {
            var localStorage = DopeElectionsApp.Instance.LocalStorage;
            var userFile = GetUserFile(cantonId, electionIds);
            var userDataString = localStorage.ReadAllText(userFile);
            JObject userJson;
            try
            {
                userJson = JObject.Parse(userDataString);
            }
            catch
            {
                userJson = new JObject();
            }

            var answers = new List<QuestionAnswer>();
            var winners = new List<QuestionCategoryWinner>();
            ProgressionData progressionData = null;

            if (userJson["answers"] is JArray answersArray)
            {
                answers.AddRange(answersArray
                    .Select(e => QuestionAnswer.TryParse(e, out var a) ? a : default)
                    .Where(a => a != null));
            }

            if (userJson["category_winners"] is JArray winnersArray)
            {
                winners.AddRange(winnersArray
                    .Select(e => QuestionCategoryWinner.TryParse(e, out var d) ? d : default)
                    .Where(d => d.CandidateId > 0));
            }

            if (userJson["progression"] is JArray progressionArray)
            {
                progressionData = ProgressionData.TryParse(progressionArray, out var raceProgressionData)
                    ? raceProgressionData
                    : null;
            }

            return new UserData(electionIds, progressionData, answers, winners);
        }

        private static Question[] LoadQuestions(string language, int electionId)
        {
            var localStorage = DopeElectionsApp.Instance.LocalStorage;
            var file = GetQuestionsFile(language, electionId);
            var data = localStorage.ReadAllText(file, true);
            return data != null ? JsonHelper.FromJsonString<Question>(data) : new Question[0];
        }

        private static QuestionAxis[] LoadQuestionAxis(string language, int electionId)
        {
            var localStorage = DopeElectionsApp.Instance.LocalStorage;
            var file = GetQuestionAxisFile(language, electionId);
            var data = localStorage.ReadAllText(file, true);
            return data != null ? JsonHelper.FromJsonString<QuestionAxis>(data) : new QuestionAxis[0];
        }

        private static QuestionCategory[] LoadQuestionCategories(string language, int electionId, Question[] questions)
        {
            var localStorage = DopeElectionsApp.Instance.LocalStorage;
            var file = GetQuestionCategoriesFile(language, electionId);
            var data = localStorage.ReadAllText(file, true);
            return data != null
                ? JsonHelper.FromJsonString<QuestionCategory>(data)
                    .Where(c => questions.Any(q => q.categoryId == c.id)).ToArray()
                : new QuestionCategory[0];
        }

        private static string GetElectionsHash(IEnumerable<int> electionIds)
        {
            return StringUtils.CreateMD5(string.Join("-", electionIds.OrderBy(id => id)));
        }

        private static string GetUserFile(int cantonId, IEnumerable<int> electionIds)
        {
            var electionsHash = GetElectionsHash(electionIds);
            return $"questionnaire/{cantonId}/{electionsHash}.json";
        }

        private static string GetQuestionsFile(string language, int electionId)
        {
            return BackendCHplusDope.GetElectionQuestionsFile(language, electionId);
        }

        private static string GetQuestionAxisFile(string language, int electionId)
        {
            return BackendCHplusDope.GetQuestionAxisFile(language, electionId);
        }

        private static string GetQuestionCategoriesFile(string language, int electionId)
        {
            return BackendCHplusDope.GetElectionCategoriesFile(language, electionId);
        }

        private class UserData
        {
            public IEnumerable<int> ElectionIds { get; }
            public ProgressionData ProgressionData { get; }
            public IEnumerable<QuestionAnswer> Answers { get; }
            public IEnumerable<QuestionCategoryWinner> CategoryWinners { get; }

            public UserData(IEnumerable<int> electionIds, ProgressionData progressionData,
                IEnumerable<QuestionAnswer> answers,
                IEnumerable<QuestionCategoryWinner> winners)
            {
                ElectionIds = electionIds;
                ProgressionData = progressionData;
                Answers = answers;
                CategoryWinners = winners;
            }

            public UserData(IEnumerable<int> electionIds)
            {
                ElectionIds = electionIds;
                Answers = new List<QuestionAnswer>();
                CategoryWinners = new List<QuestionCategoryWinner>();
            }
        }
    }
}