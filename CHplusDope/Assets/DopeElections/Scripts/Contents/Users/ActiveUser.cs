using System;
using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using DopeElections.Progression;
using DopeElections.Questions;
using Essentials;
using Newtonsoft.Json.Linq;
using Popups;
using Progression;
using UnityEngine;

namespace DopeElections.Users
{
    public class ActiveUser
    {
        public delegate void QuestionnaireEvent(Questionnaire questionnaire);

        public delegate void CandidateEvent(Candidate candidate);

        public event CandidateEvent CandidateConfigurationChanged = delegate { };
        public event QuestionnaireEvent QuestionnaireChanged = delegate { };

        private const string File = "user.json";

        public User User { get; private set; }

        public int CantonId { get; set; }
        public int ConstituencyId { get; set; }
        public int ElectionId { get; set; }
        public int ListId { get; set; }

        public SmartSpider SmartSpider { get; set; }
        public SmartSpider PreviousSmartSpider { get; set; }
        public UserJourneyTree UserJourney { get; set; }
        public Questionnaire Questionnaire { get; private set; }

        public NamespacedKey FaceId { get; set; }

        public List<int> LikedCandidates { get; } = new List<int>();
        public List<int> DislikedCandidates { get; } = new List<int>();
        private List<CandidateUserData> CandidateUserData { get; } = new List<CandidateUserData>();

        public Dictionary<int, int>[] SmartSpiderAnswers;

        public ActiveUser() : this(new User(), new SmartSpider())
        {
        }

        public ActiveUser(User user, SmartSpider smartSpider)
        {
            User = user;
            SmartSpider = smartSpider;
            SmartSpiderAnswers = new Dictionary<int, int>[8];
        }

        public void LikeCandidate(Candidate candidate, bool like)
        {
            if (like && !LikedCandidates.Contains(candidate.id)) LikedCandidates.Add(candidate.id);
            else if (!like) LikedCandidates.Remove(candidate.id);
            if (like && DislikedCandidates.Contains(candidate.id))
            {
                DislikedCandidates.Remove(candidate.id);
            }

            Save();

            CandidateConfigurationChanged(candidate);
        }

        public void ChooseElection()
        {
            var cantonId = CantonId;
            var districtId = ConstituencyId;

            var assets = DopeElectionsApp.Instance.Assets;
            // fetch canton
            var canton = assets.GetAsset<Canton>(cantonId);
            // fetch canton and municipality constituencies, sort by type so canton constituency is first
            var allConstituencies = assets.GetAssets<Constituency>();
            var constituencies = allConstituencies
                .Where(c => c.cantonId == canton.id && (c.type == Constituency.Type.Canton || c.id == districtId))
                .OrderBy(c => c.type)
                .ToArray();
            // fetch all elections currently happening anywhere
            var allElections = DopeElectionsApp.Instance.Assets.GetAssets<Election>();
            // filter by elections happening in this canton
            var elections = constituencies
                .SelectMany(c => allElections.Where(e => e.constituencyId == c.id))
                .ToArray();
            // pick first election
            var election = elections.FirstOrDefault();
            // apply election id if present
            ElectionId = election != null ? election.electionId : 0;
        }

        public void DislikeCandidate(Candidate candidate, bool dislike)
        {
            if (dislike && !DislikedCandidates.Contains(candidate.id)) DislikedCandidates.Add(candidate.id);
            else if (!dislike) DislikedCandidates.Remove(candidate.id);
            if (dislike && LikedCandidates.Contains(candidate.id))
            {
                LikedCandidates.Remove(candidate.id);
            }

            Save();

            CandidateConfigurationChanged(candidate);
        }

        public void UpdateSmartSpider()
        {
            var axisMatrix = new Dictionary<int, int>[8];
            for (var i = 0; i < axisMatrix.Length; i++)
            {
                axisMatrix[i] = new Dictionary<int, int>();
            }

            var questions = Questionnaire.Questions;

            foreach (var questionAnswer in Questionnaire.Progression.UserAnswers)
            {
                var question = questions.FirstOrDefault(element => element.id == questionAnswer.questionId);
                if (question == null)
                {
                    continue;
                }

                var questionAxis = question.axis;
                foreach (var link in questionAxis)
                {
                    var value = link.value == 1 ? questionAnswer.answer : 100 - questionAnswer.answer;
                    axisMatrix[link.axis - 1][questionAnswer.questionId] = value;
                }
            }

            SmartSpiderAnswers = axisMatrix;

            SmartSpider = SmartSpider.RecalculateSmartSpider(SmartSpiderAnswers);
            //Debug.Log("update done ");
        }

        public void ReloadQuestionnaire()
        {
            if (CantonId == 0 || ConstituencyId == 0)
            {
                Questionnaire = null;
                QuestionnaireChanged(null);
                return;
            }

            var canton = this.GetCanton();
            var constituency = this.GetConstituency();
            var election = this.GetElection();
            Questionnaire = Questionnaire.Load(canton, constituency, election);
            foreach (var candidate in this.GetRegionalCandidates())
            {
                candidate.RecalculateMatch();
                candidate.RecalculateSmartSpider();
            }

            QuestionnaireChanged(Questionnaire);
        }

        public CandidateUserData GetOrCreateCandidateUserData(int candidateId)
        {
            var existing = GetCandidateUserData(candidateId);
            if (existing != null) return existing;

            var result = new CandidateUserData(candidateId);
            CandidateUserData.Add(result);
            return result;
        }

        public CandidateUserData GetCandidateUserData(int candidateId)
        {
            return CandidateUserData.FirstOrDefault(d => d.CandidateId == candidateId);
        }

        public void RemoveCandidateUserData(int candidateId)
        {
            var existing = GetCandidateUserData(candidateId);
            if (existing != null) CandidateUserData.Remove(existing);
        }

        public void Save()
        {
            var data = new JObject()
            {
                ["user"] = User.ToString(),
                ["spider"] = SmartSpider.ToString(),
                ["user_journey"] = UserJourney.Save().Serialize(),
                ["face"] = FaceId.ToString(),
                ["canton"] = CantonId,
                ["constituency"] = ConstituencyId,
                ["election"] = ElectionId,
                ["list"] = ListId,
                ["liked"] = new JArray(LikedCandidates.Cast<object>().ToArray()),
                ["disliked"] = new JArray(DislikedCandidates.Cast<object>().ToArray()),
                ["candidates"] = new JArray(CandidateUserData.Select(d => d.Serialize()).Cast<object>().ToArray())
            };
            DopeElectionsApp.Instance.LocalStorage.WriteAllText(File, data.ToString());

            if (Questionnaire != null)
            {
                QuestionnaireSerializer.Save(Questionnaire);
            }
        }

        public void Load()
        {
            var json = DopeElectionsApp.Instance.LocalStorage.GetJson(File);
            if (json == null) return;

            try
            {
                User = json["user"] != null ? JsonUtility.FromJson<User>((string) json["user"]) : new User();
            }
            catch (Exception e)
            {
                Debug.LogError("Could not load user!\n" + e);
                User = new User();
            }

            try
            {
                SmartSpider = json["spider"] != null
                    ? JsonUtility.FromJson<SmartSpider>((string) json["spider"])
                    : new SmartSpider();
            }
            catch (Exception e)
            {
                Debug.LogError("Could not load smart spider!\n" + e);
                SmartSpider = new SmartSpider();
            }

            ElectionId = json["election"] != null ? (int) json["election"] : 0;
            FaceId = json["face"] != null && NamespacedKey.TryParse((string) json["face"], out var faceId)
                ? faceId
                : default;
            CantonId = json["canton"] != null ? (int) json["canton"] : 0;
            ConstituencyId = json["constituency"] != null ? (int) json["constituency"] : 0;
            ListId = json["list"] != null ? (int) json["list"] : 0;

            var userJourneySection = json["user_journey"];

            var likedArray = json["liked"] as JArray;
            var dislikedArray = json["disliked"] as JArray;
            var candidatesArray = json["candidates"] as JArray;

            if (likedArray != null) LikedCandidates.AddRange(likedArray.Select(e => (int) e));
            if (dislikedArray != null) DislikedCandidates.AddRange(dislikedArray.Select(e => (int) e));

            if (candidatesArray != null)
            {
                CandidateUserData.AddRange(candidatesArray
                    .Select(e => Users.CandidateUserData.TryParse(e, out var d) ? d : default)
                    .Where(d => d != null));
            }

            if (ProgressionData.TryParse(userJourneySection, out var userJourneyData))
            {
                UserJourney.Load(userJourneyData);
            }

            ReloadQuestionnaire();
        }
    }
}