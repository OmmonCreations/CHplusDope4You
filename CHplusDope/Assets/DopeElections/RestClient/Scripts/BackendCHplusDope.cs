using System;
using System.IO;
using DopeElections.Answer;
using DopeElections.Request;
using Newtonsoft.Json.Linq;
using Proyecto26;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace DopeElections
{
    public class BackendCHplusDope
    {
        private void LogMessage(string title, string message)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog(title, message, "Ok");
#else
		Debug.Log(message);
#endif
        }

        public string Language { get; set; }

        #region Requests

        #region Elections

        private const string ExampleDataPath = "example";

        public void GetElections(Action<string> callback)
        {
            var path = Path.Combine(ExampleDataPath, $"{Language}/elections.json");
            DopeElectionsApp.Instance.InternalStorage.ReadAllText(path, callback);
        }

        public void GetElectionQuestions(int electionId, Action<string> callback)
        {
            var path = Path.Combine(ExampleDataPath, $"{Language}/election/{electionId}/questions.json");
            DopeElectionsApp.Instance.InternalStorage.ReadAllText(path, callback);
        }

        public void GetElectionCategories(int electionId, Action<string> callback)
        {
            var path = Path.Combine(ExampleDataPath, $"{Language}/election/{electionId}/categories.json");
            DopeElectionsApp.Instance.InternalStorage.ReadAllText(path, callback);
        }

        public void GetElectionCandidates(int electionId, Action<string> callback)
        {
            var path = Path.Combine(ExampleDataPath, $"{Language}/election/{electionId}/candidates.json");
            DopeElectionsApp.Instance.InternalStorage.ReadAllText(path, callback);
        }

        public void GetCantons(Action<string> callback)
        {
            var path = Path.Combine(ExampleDataPath, "cantons.json");
            DopeElectionsApp.Instance.InternalStorage.ReadAllText(path, callback);
        }

        public void GetConstituencies(Action<string> callback)
        {
            var path = Path.Combine(ExampleDataPath, $"{Language}/constituencies.json");
            DopeElectionsApp.Instance.InternalStorage.ReadAllText(path, callback);
        }

        public void GetParties(Action<string> callback)
        {
            var path = Path.Combine(ExampleDataPath, $"{Language}/parties.json");
            DopeElectionsApp.Instance.InternalStorage.ReadAllText(path, callback);
        }

        public void GetQuestionAxis(int electionId, Action<string> callback)
        {
            var path = Path.Combine(ExampleDataPath, $"{Language}/election/{electionId}/question_axis.json");
            DopeElectionsApp.Instance.InternalStorage.ReadAllText(path, callback);
        }

        public void GetElectionResponses(int electionId, Action<string> callback)
        {
            var path = Path.Combine(ExampleDataPath, $"{Language}/election/{electionId}/responses.json");
            DopeElectionsApp.Instance.InternalStorage.ReadAllText(path, callback);
        }

        #endregion

        #endregion

        #region Filenames

        public const string DownloadsDirectory = "downloads";

        public static string GetCantonsFile()
        {
            return Path.Combine(DownloadsDirectory, "cantons.json");
        }

        public static string GetPartiesFile(string language)
        {
            return Path.Combine(DownloadsDirectory, $"{language}/parties.json");
        }

        public static string GetConstituenciesFile(string language)
        {
            return Path.Combine(DownloadsDirectory, $"{language}/constituencies.json");
        }

        public static string GetElectionsFile(string language)
        {
            return Path.Combine(DownloadsDirectory, $"{language}/elections.json");
        }

        public static string GetElectionPath(string language, int electionId)
        {
            return Path.Combine(DownloadsDirectory, $"{language}/election/{electionId}");
        }

        public static string GetElectionQuestionsFile(string language, int electionId)
        {
            return Path.Combine(GetElectionPath(language, electionId), "questions.json");
        }

        public static string GetElectionCategoriesFile(string language, int electionId)
        {
            return Path.Combine(GetElectionPath(language, electionId), "categories.json");
        }

        public static string GetElectionCandidatesFile(string language, int electionId)
        {
            return Path.Combine(GetElectionPath(language, electionId), "candidates.json");
        }

        public static string GetElectionListsFile(string language, int electionId)
        {
            return Path.Combine(GetElectionPath(language, electionId), "lists.json");
        }

        public static string GetQuestionAxisFile(string language, int electionId)
        {
            return Path.Combine(GetElectionPath(language, electionId), "question_axis.json");
        }

        public static string GetElectionResponsesFile(string language, int electionId)
        {
            return Path.Combine(GetElectionPath(language, electionId), "responses.json");
        }

        #endregion

        public static string GetLanguageCode(string language)
        {
            switch (language)
            {
                case "de": return "de_CH";
                case "fr": return "fr_CH";
                case "it": return "it_CH";
                case "rm": return "rm_CH";
                case "en": return "en_CH";
                default: return "en_CH";
            }
        }
    }
}