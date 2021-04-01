using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Races.RaceTracks;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    public class RaceCandidate
    {
        public delegate void ReactionEvent(GroupReactionContext context, ReactionData data, CompiledPath reactionPath);

        public delegate void GroupEvent(CandidateGroup group);

        public delegate void SlotEvent(CandidateSlot slot);

        public delegate void AnchorEvent(Vector2 anchor);

        public delegate void CandidateEvent();

        public event ReactionEvent Reacted = delegate { };
        public event GroupEvent FollowedGroup = delegate { };
        public event CandidateEvent Resetted = delegate { };

        /// <summary>
        /// The wrapped candidate
        /// </summary>
        public Candidate Candidate { get; }

        public RaceCandidateConfiguration Configuration { get; }

        public AgreementState AgreementState { get; private set; }

        /// <summary>
        /// The current category agreement score, where each answer is added based on the distance to the user answer
        /// 100% agreement is represented as 100 points per question, 0% agreement as 0 points.
        /// </summary>
        public int AgreementScore => AgreementState.AgreementScore;

        /// <summary>
        /// The current category match from 0 to 1
        /// </summary>
        public float CategoryMatch => AgreementState.CategoryMatch;

        /// <summary>
        /// The current candidate group slot
        /// </summary>
        public CandidateSlot Slot { get; set; }

        /// <summary>
        /// The current preferred position in the candidate group where x is the relative sideways position on the
        /// race track and y is the absolute distance to the track start
        /// </summary>
        public RaceTrackVector GroupAnchor { get; set; }

        public bool IsAlive { get; set; }
        
        public CandidateReactionContext LastReactionContext { get; set; }

        public int id => Candidate.id;
        public string fullName => firstName + " " + lastName;
        public string firstName => Candidate.firstName;
        public string lastName => Candidate.lastName;
        public float match => Candidate.match;
        public SmartSpider smartSpider => Candidate.smartSpider;
        public float BaseSpeed => Configuration.RunSpeed;

        public RaceCandidate(Candidate candidate, RaceCandidateConfiguration configuration)
        {
            Candidate = candidate;
            Configuration = configuration;
        }

        public void ResetState()
        {
            Resetted();
        }

        public void UpdateAgreementScore(QuestionMarathon marathon, bool includeCurrentQuestion = false)
        {
            if (marathon == null)
            {
                AgreementState = new AgreementState(0, 0);
                return;
            }

            var questionCount = marathon.CurrentQuestionIndex + (includeCurrentQuestion ? 1 : 0);
            var questions = marathon.Questions.Take(questionCount).ToList();
            AgreementState = CalculateAgreement(questions);

            // if (CategoryMatch >= 0.75f) Debug.Log("Agreement: " + AgreementScore + ", Match: " + CategoryMatch);
            /*
            var userAnswers = DopeElectionsApp.Instance.User.QuestionsAnswers
                .Where(a => questions.All(q => q.id != a.questionId));
            AgreementScore = Candidate.responses
                .Sum(r => userAnswers
                    .Where(a => a.questionId == r.questionId)
                    .Select(a => Mathf.Abs(a.answer - r.value))
                    .DefaultIfEmpty(0)
                    .First()
                );
                */
            // AgreementScore = 0;
        }

        public AgreementState CalculateAgreement(IEnumerable<Question> questions)
        {
            var userAnswers = DopeElectionsApp.Instance.User.Questionnaire.Progression.UserAnswers;
            var entries = questions.Select(q => new KeyValuePair<Question, QuestionAnswer>(
                q,
                userAnswers.FirstOrDefault(a => a.questionId == q.id)
            )).Where(e => e.Value != null && e.Value.answer >= 0);
            return CalculateAgreement(entries);
        }

        private AgreementState CalculateAgreement(IEnumerable<KeyValuePair<Question, QuestionAnswer>> questions)
        {
            return CalculateAgreement(questions, Candidate.responses);
        }

        private static AgreementState CalculateAgreement(IEnumerable<KeyValuePair<Question, QuestionAnswer>> entries,
            IEnumerable<Response> responses)
        {
            var relevantCandidateResponses = responses
                .Where(r => entries.Any(a => a.Key.id == r.questionId))
                .ToArray();
            var agreementAnswerMap = entries.Select(e => new KeyValuePair<QuestionAnswer, Response>(
                    e.Value,
                    relevantCandidateResponses.FirstOrDefault(r => r.questionId == e.Key.id)
                )).Where(e => e.Key != null && e.Value != null && e.Value.value >= 0)
                .ToDictionary(e => e.Key, e => e.Value);
            var matchAnswerMap = agreementAnswerMap.ToDictionary(e => e.Key, e => 1f);

            var agreementScore = agreementAnswerMap
                .Where(e => e.Value != null && e.Value.value >= 0)
                .Select(e => 100 - Mathf.Abs(e.Key.answer - e.Value.value))
                .Sum();

            var categoryMatch = Candidate.CalculateMatch(matchAnswerMap, relevantCandidateResponses);
            return new AgreementState(agreementScore, categoryMatch);
        }

        public void FollowGroup(CandidateGroup group)
        {
            FollowedGroup(group);
        }

        public void ReactToAnswer(GroupReactionContext context, ReactionData data, CompiledPath reactionPath)
        {
            AgreementState = data.AgreementState;
            GroupAnchor = data.GroupAnchor;
            IsAlive = data.IsAlive;
            Reacted(context, data, reactionPath);
        }
    }
}