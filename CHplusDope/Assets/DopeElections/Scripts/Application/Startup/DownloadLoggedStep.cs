using System;
using System.Globalization;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using FileStore;
using Popups;
using Proyecto26;
using RuntimeAssetImporter;
using UnityEngine;

namespace DopeElections.Startup
{
    public class DownloadLoggedStep : ProgressDisplayingStep
    {
        private LocalStorage Storage { get; set; }
        private BackendCHplusDope RestApi { get; set; }
        private AssetPack Assets { get; set; }

        private string LanguageCode { get; set; }

        private Election[] LocallyStoredElections { get; set; }

        private bool _electionsLoaded;
        private bool _partiesLoaded;

        private bool[] _questionsDownloaded;
        private bool[] _categoriesDownloaded;
        private bool[] _candidateDownloaded;
        private bool[] _questionAxisDownloaded;
        private bool[] _candidateResponseDownloaded;

        private bool _error;

        public DownloadLoggedStep(DopeElectionsApp app, float progressStart, float progressEnd) : base(app,
            progressStart, progressEnd)
        {
        }

        public override void Run(bool data)
        {
            if (!data)
            {
                Complete(false);
                return;
            }

            // Debug.Log("Download logged value");
            Storage = App.LocalStorage;
            RestApi = App.RestApi;
            Assets = App.Assets;
            LanguageCode = BackendCHplusDope.GetLanguageCode(App.Settings.GetValue(Setting.Language));

            LocallyStoredElections = LoadElectionsFromDisk();

            StartDownload();
        }

        private void StartDownload()
        {
            base.Run(true);
            Progress = 0;
            ProgressLabel = LKey.Views.Startup.Downloading;
            DownloadElections(LoadElectionAssets);
            DownloadParties();
            UpdateProgress();
        }

        #region Load Elections

        private void DownloadElections(Action<Election[]> callback)
        {
            var file = BackendCHplusDope.GetElectionsFile(LanguageCode);

            RestApi.GetElections(
                data =>
                {
                    if (data == null && !IsPresent(file))
                    {
                        CallError(ErrorSource.Elections);
                        return;
                    }

                    if (data != null) Storage.WriteAllText(file, data, true);
                    callback(LoadElections());
                });
        }

        private Election[] LoadElectionsFromDisk()
        {
            try
            {
                var file = BackendCHplusDope.GetElectionsFile(LanguageCode);
                if (!IsPresent(file))
                {
                    return new Election[0];
                }

                var electionContent = Storage.ReadAllText(file, true);

                return electionContent != null ? JsonHelper.FromJsonString<Election>(electionContent) : new Election[0];
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return new Election[0];
            }
        }

        private Election[] LoadElections()
        {
            try
            {
                var elections = LoadElectionsFromDisk();
                foreach (var e in elections)
                {
                    var dateParts = e.electionDate.Split('-');
                    if (dateParts.Length != 3) continue;

                    var year = int.TryParse(dateParts[0], out var y) ? y : 0;
                    var month = int.TryParse(dateParts[1], out var m) ? m : 0;
                    var day = int.TryParse(dateParts[2], out var d) ? d : 0;
                    if (year == 0 || month == 0 || day == 0) continue;

                    var electionDate = new DateTime(year, month, day);
                    e.electionTimestamp = Math.Max(0, (long) (electionDate - new DateTime(1970, 1, 1)).TotalSeconds);
                }

                _questionsDownloaded = new bool[elections.Length];
                _categoriesDownloaded = new bool[elections.Length];
                _candidateDownloaded = new bool[elections.Length];
                _questionAxisDownloaded = new bool[elections.Length];
                _candidateResponseDownloaded = new bool[elections.Length];

                Assets.PutAssets(elections);
                _electionsLoaded = true;

                return elections;
            }
            catch (Exception e)
            {
                Debug.LogError("Error while loading elections:\n" + e);
                _electionsLoaded = true;
                return new Election[0];
            }
        }

        #endregion

        #region Party Loading

        private void DownloadParties()
        {
            var file = BackendCHplusDope.GetPartiesFile(LanguageCode);
            var partiesData = Storage.FileExists(file) ? Storage.ReadAllText(file, true) : null;
            if (partiesData != null)
            {
                LoadParties(partiesData);
                return;
            }

            RestApi.GetParties(data =>
            {
                if (data == null)
                {
                    CallError(ErrorSource.Parties);
                    return;
                }

                Storage.WriteAllText(file, data, true);
                LoadParties(data);
            });
        }

        private void LoadParties(string partiesData)
        {
            var parties = partiesData != null ? JsonHelper.FromJsonString<Party>(partiesData) : null;

            if (parties == null)
            {
                _partiesLoaded = true;
                Debug.LogWarning("No data found. Loaded 0 parties!");
                return;
            }

            Assets.PutAssets(parties);
            _partiesLoaded = true;
            UpdateProgress();
        }

        #endregion

        #region Election Assets

        private void LoadElectionAssets(Election[] elections)
        {
            DownloadQuestions(elections);
            DownloadQuestionCategories(elections);
            DownloadCandidates(elections);
            DonwloadQuestionAxis(elections);
            DonwloadCandidateResponses(elections);
        }

        private void DownloadQuestionCategories(Election[] elections)
        {
            for (var i = 0; i < elections.Length; i++)
            {
                var index = i;
                var election = elections[i];
                var file = BackendCHplusDope.GetElectionCategoriesFile(LanguageCode, election.id);
                if (IsLatestVersion(file, election))
                {
                    _categoriesDownloaded[index] = true;
                    continue;
                }

                RestApi.GetElectionCategories(election.id, data =>
                {
                    if (data == null)
                    {
                        CallError(ErrorSource.ElectionCategories);
                        return;
                    }

                    _categoriesDownloaded[index] = true;
                    Storage.WriteAllText(file, data, true);
                    // Debug.Log("Election Questions downloaded!");
                    // if (_questionDownloaded.All(b => b)) Debug.Log("Categories downloaded!");
                    UpdateProgress();
                });
            }

            UpdateProgress();
        }


        private void DownloadQuestions(Election[] elections)
        {
            for (var i = 0; i < elections.Length; i++)
            {
                var index = i;
                var election = elections[i];
                var file = BackendCHplusDope.GetElectionQuestionsFile(LanguageCode, election.id);
                if (IsLatestVersion(file, election))
                {
                    _questionsDownloaded[index] = true;
                    continue;
                }

                RestApi.GetElectionQuestions(election.id, data =>
                {
                    if (data == null)
                    {
                        CallError(ErrorSource.ElectionQuestions);
                        return;
                    }

                    _questionsDownloaded[index] = true;
                    Storage.WriteAllText(file, data, true);
                    // Debug.Log("Election Questions downloaded!");
                    // if (_questionDownloaded.All(b => b)) Debug.Log("Questions downloaded!");
                    UpdateProgress();
                });
            }

            UpdateProgress();
        }

        private void DownloadCandidates(Election[] elections)
        {
            for (var i = 0; i < elections.Length; i++)
            {
                var index = i;
                var election = elections[i];
                var file = BackendCHplusDope.GetElectionCandidatesFile(LanguageCode, election.id);
                if (IsLatestVersion(file, election))
                {
                    _candidateDownloaded[index] = true;
                    continue;
                }

                RestApi.GetElectionCandidates(election.id, data =>
                {
                    if (data == null)
                    {
                        CallError(ErrorSource.ElectionCandidates);
                        return;
                    }

                    _candidateDownloaded[index] = true;
                    Storage.WriteAllText(file, data, true);
                    // Debug.Log("Election Candidates downloaded!");
                    // if (_candidateDownloaded.All(b => b)) Debug.Log("Candidates downloaded!");
                    UpdateProgress();
                });
            }

            UpdateProgress();
        }

        private void DonwloadQuestionAxis(Election[] elections)
        {
            for (var i = 0; i < elections.Length; i++)
            {
                var index = i;
                var election = elections[i];
                var file = BackendCHplusDope.GetQuestionAxisFile(LanguageCode, election.id);
                if (IsLatestVersion(file, election))
                {
                    _questionAxisDownloaded[index] = true;
                    continue;
                }

                RestApi.GetQuestionAxis(election.id, data =>
                {
                    if (data == null)
                    {
                        CallError(ErrorSource.QuestionAxis);
                        return;
                    }

                    _questionAxisDownloaded[index] = true;
                    Storage.WriteAllText(file, data, true);
                    // Debug.Log("Election Questions downloaded!");
                    // if (_questionDownloaded.All(b => b)) Debug.Log("Questions downloaded!");
                    UpdateProgress();
                });
            }

            UpdateProgress();
        }

        private void DonwloadCandidateResponses(Election[] elections)
        {
            for (var i = 0; i < elections.Length; i++)
            {
                var index = i;
                var election = elections[i];
                var file = BackendCHplusDope.GetElectionResponsesFile(LanguageCode, election.id);
                if (IsLatestVersion(file, election))
                {
                    _candidateResponseDownloaded[index] = true;
                    continue;
                }

                RestApi.GetElectionResponses(election.id, data =>
                {
                    if (data == null)
                    {
                        CallError(ErrorSource.ElectionResponses);
                        return;
                    }

                    _candidateResponseDownloaded[index] = true;
                    Storage.WriteAllText(file, data, true);
                    UpdateProgress();
                });
            }

            UpdateProgress();
        }

        #endregion

        #region Helpers

        private bool IsPresent(string file)
        {
            return App.LocalStorage.FileExists(file);
        }

        private bool IsLatestVersion(string file, Election election)
        {
            var locallyStoredElection = LocallyStoredElections.FirstOrDefault(e => e.id == election.id);
            var result = locallyStoredElection != null &&
                         locallyStoredElection.updatedAt == election.updatedAt &&
                         !string.IsNullOrWhiteSpace(election.updatedAt);

            return result && Storage.ReadAllText(file, true) != null;
        }

        private void UpdateProgress()
        {
            const float step = 1f / 7;

            var partiesProgress = _partiesLoaded ? step : 0;
            var electionsProgress = _electionsLoaded ? step : 0;

            var questionsProgress = GetProgress(_questionsDownloaded, step);
            var categoriesProgress = GetProgress(_categoriesDownloaded, step);
            var candidatessProgress = GetProgress(_candidateDownloaded, step);
            var questionAxisProgress = GetProgress(_questionAxisDownloaded, step);
            var candidateResponsesProgress = GetProgress(_candidateResponseDownloaded, step);

            var progress = partiesProgress + electionsProgress + questionsProgress + candidatessProgress +
                           categoriesProgress + questionAxisProgress + candidateResponsesProgress;

            Progress = progress;

            if (progress < 1 || _error)
            {
                return;
            }

            // Debug.Log("Download complete!");
            Complete(true);
        }

        private static float GetProgress(bool[] tracker, float step)
        {
            return tracker == null
                ? 0
                : tracker.Length > 0
                    ? tracker.Count(q => q) / (float) tracker.Length * step
                    : step;
        }

        private void CallError(ErrorSource source)
        {
            Debug.LogWarning("Error downloading " + source);
            if (_error) return;
            _error = true;
            App.Popups.ShowPopup(new AlertPopup(
                LKey.Views.Startup.DownloadFailedAlert.Title,
                LKey.Views.Startup.DownloadFailedAlert.Text
            ).Then(() => Complete(false)));
        }

        private enum ErrorSource
        {
            Elections,
            Parties,
            ElectionCategories,
            ElectionQuestions,
            ElectionCandidates,
            ElectionLists,
            QuestionAxis,
            ElectionResponses
        }

        #endregion
    }
}