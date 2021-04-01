using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AsyncListeners;
using DopeElections.Answer;
using DopeElections.Localizations;
using FileStore;
using Proyecto26;
using RuntimeAssetImporter;
using UnityEngine;

namespace DopeElections.Startup
{
    public class LoadAssetsStep : ProgressDisplayingStep
    {
        private BackendCHplusDope RestApi { get; set; }
        private LocalStorage Storage { get; set; }
        private AssetPack Assets { get; set; }
        private string LanguageCode { get; set; }

        private Election[] Elections { get; set; }

        private Canton[] _cantons;
        private Constituency[] _constituencies;
        private Candidate[] _candidates;
        private Dictionary<Election, ElectionList[]> _lists;

        private bool _done;

        public LoadAssetsStep(DopeElectionsApp app, float progressStart, float progressEnd) : base(app, progressStart,
            progressEnd)
        {
        }

        public override void Run(bool data)
        {
            base.Run(data);
            if (!data)
            {
                Complete(false);
                return;
            }

            Progress = 0;
            LanguageCode = BackendCHplusDope.GetLanguageCode(App.Settings.GetValue(Setting.Language));
            LoadAssets(Complete);
        }

        private void LoadAssets(Action<bool> callback)
        {
            Storage = App.LocalStorage;
            RestApi = App.RestApi;
            Assets = App.Assets;

            Elections = Assets.GetAssets<Election>();

            ProgressLabel = LKey.Views.Startup.LoadLocalFiles;

            foreach (var l in App.BuiltinAssets) l.LoadInto(Assets);

            var thread = new Thread(LoadAssets);
            thread.Start();

            AsyncOperations.Await(() => _done).OnComplete += () => { callback(true); };
        }

        private void PushAssets()
        {
            var assets = Assets;
            lock (assets)
            {
                if (_cantons != null) assets.PutAssets(_cantons);
                if (_constituencies != null) assets.PutAssets(_constituencies);
                if (_candidates != null) assets.PutAssets(_candidates);
                if (_lists != null)
                {
                    foreach (var e in _lists)
                    {
                        var election = e.Key;
                        var entries = e.Value;
                        assets.PutAssets(entries);
                        assets.PutAsset(new ElectionListMap(election.id,
                            entries.Select(l => l.id)));
                    }
                }
            }
        }

        /// <summary>
        /// Executed in separate thread!
        /// </summary>
        private void LoadAssets()
        {
            // Debug.Log("Loading assets...");
            _cantons = LoadCantons();
            Progress = 0.2f;
            _constituencies = LoadConstituencies();
            Progress = 0.4f;

            Progress = 0.6f;
            _candidates = LoadCandidates();
            Progress = 0.95f;

            PushAssets();
            Progress = 1;

            _done = true;
            // Debug.Log("Assets loaded!");
        }

        private Canton[] LoadCantons()
        {
            var filename = BackendCHplusDope.GetCantonsFile();

            var cantonsData = Storage.ReadAllText(filename, true);

            var cantons = cantonsData != null ? JsonHelper.FromJsonString<Canton>(cantonsData) : null;

            if (cantons == null)
            {
                Debug.LogWarning("No data found. Loaded 0 canton!");
                return null;
            }

            return cantons;
        }

        private Constituency[] LoadConstituencies()
        {
            var filename = BackendCHplusDope.GetConstituenciesFile(LanguageCode);

            var constituencyContent = Storage.ReadAllText(filename, true);

            var constituencies = constituencyContent != null
                ? JsonHelper.FromJsonString<Constituency>(constituencyContent)
                : null;

            if (constituencies == null)
            {
                Debug.LogWarning("No data found. Loaded 0 constituency !");
                return null;
            }

            return constituencies;
        }

        private Candidate[] LoadCandidates()
        {
            var elections = Elections;

            var candidates = new Dictionary<int, Candidate>();

            foreach (var election in elections)
            {
                var candidatesFile = BackendCHplusDope.GetElectionCandidatesFile(LanguageCode, election.id);
                var electionCandidateContent = Storage.ReadAllText(candidatesFile, true);

                var electionCandidates = electionCandidateContent != null
                    ? JsonHelper.FromJsonString<Candidate>(electionCandidateContent)
                    : null;

                if (electionCandidates == null)
                {
                    Debug.LogWarning("No candidates found. Loaded 0 candidates!");
                    return null;
                }

                election.candidates = electionCandidates.Select(c => c.id).ToArray();

                var responsesFile = BackendCHplusDope.GetElectionResponsesFile(LanguageCode, election.id);
                var candidateResponsesContent = Storage.ReadAllText(responsesFile, true);

                var currentCandidatesResponses = candidateResponsesContent != null
                    ? JsonHelper.FromJsonString<Response>(candidateResponsesContent)
                    : null;

                if (currentCandidatesResponses == null) continue;
                
                foreach (var candidate in electionCandidates)
                {
                    var responses =
                        currentCandidatesResponses.Where(a => a.candidateId == candidate.id).ToArray();
                    candidate.responses = responses;
                    //TEMPORARY fix of bug from backend (more that one candidate with the same id in the file)
                    candidates[candidate.id] = candidate;
                }
            }

            return candidates.OrderBy(e => e.Key).Select(e => e.Value).ToArray();
        }
    }
}