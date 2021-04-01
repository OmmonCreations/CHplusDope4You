using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.ObstacleCourses;
using DopeElections.Races.GroupLayout;
using DopeElections.Races.RaceTracks;
using Essentials;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Calculates the result state of the group and the circumstances for the obstacle course after the user
    /// answers a question
    /// </summary>
    public class GroupReactionResult
    {
        public CandidateGroup Group { get; }
        public IObstacleCourseGenerator Generator { get; }
        public float ObstacleCourseWidth { get; private set; }
        public float ObstacleCourseLength { get; private set; }
        public float TargetPosition { get; private set; }
        public int MaxAgreementScore { get; private set; }
        public CandidateGroupLayout TargetLayout { get; private set; }
        public CandidateSubgroup[] Groups { get; private set; }

        public CandidateSlotMap SlotMap { get; } = new CandidateSlotMap();
        public CandidateVectorMap GroupAnchorMap { get; } = new CandidateVectorMap();
        public CandidateAgreementMap AgreementMap { get; } = new CandidateAgreementMap();
        public CandidateAgreementStateMap AgreementStatesMap { get; } = new CandidateAgreementStateMap();
        public CandidateReactionMap ReactionMap { get; } = new CandidateReactionMap();

        private GroupReactionResult(CandidateGroup group, IObstacleCourseGenerator generator)
        {
            Group = group;
            Generator = generator;
        }

        #region Logic

        /// <summary>
        /// Calculates how much each candidate agrees with the users answer
        /// </summary>
        private void CalculcateAgreements(Question[] questions, QuestionAnswer answer)
        {
            var candidates = Group.Candidates;

            var question = questions.Last();
            var questionId = question.id;
            var userAnswer = answer.answer;

            var agreementMap = AgreementMap;
            var agreementStatesMap = AgreementStatesMap;

            foreach (var candidate in candidates)
            {
                var response = candidate.Candidate.responses.FirstOrDefault(c => c.questionId == questionId);
                var candidateAnswer = response != null ? response.value : 0;
                var agreement = candidateAnswer >= 0 ? 100 - Mathf.Abs(userAnswer - candidateAnswer) : -1;
                var agreementState = candidate.CalculateAgreement(questions);
                agreementMap[candidate] = agreement;
                agreementStatesMap[candidate] = agreementState;
            }
        }

        private void CalculateMaxAgreement()
        {
            MaxAgreementScore = Group.MaxScore + 100;
        }

        /// <summary>
        /// Calculates candidate subgroups based on their shared category agreement score after the user's answer
        /// </summary>
        private void CalculateTargetSubgroups()
        {
            var context = Group.Context;
            var race = Group.Race;
            var agreementScoreMap = AgreementStatesMap;
            var composition = Group.Composition;
            var relativeRaceIndex = Group.Context.RelativeRaceIndex;
            var currentRaceProgress = race.Progress + race.ProgressStepSize;

            // Debug.Log(string.Join("\n", AgreementStatesMap.Select(e => e.Key.fullName + ": " + e.Value)));
            // Debug.Log("Min: " + minAgreementScore + ", Max: " + maxAgreementScore + ", Threshold: " + matchThreshold);

            Groups = GroupCompositionSolver.CalculateGroups(context, agreementScoreMap, composition, relativeRaceIndex,
                currentRaceProgress);
        }

        /// <summary>
        /// Calculates the candidate group's layout after the reactions have played out
        /// using the most recent track width
        /// </summary>
        private void CalculateTargetLayout()
        {
            var group = Group;
            var obstacleCourseWidth = group.RaceTrack.Width;
            var layout = GroupLayoutSolver.CalculateLayoutFixedWidth(group, obstacleCourseWidth, Groups);
            var slotMap = SlotMap;
            var rows = layout.Rows;
            for (var y = 0; y < rows.Length; y++)
            {
                var row = rows[y];
                var candidates = row.Candidates;
                for (var x = 0; x < candidates.Length; x++)
                {
                    var candidate = candidates[x];
                    if (candidate == null) continue;
                    slotMap[candidate] = new CandidateSlot(x, y);
                }
            }

            ObstacleCourseWidth = obstacleCourseWidth;
            TargetLayout = layout;
        }

        /// <summary>
        /// Calculates the candidate group's target position based on the layout after the reactions have played out.
        /// Leaves some space between the end of the target layout and the group's current position to allow for the
        /// insertion of an obstacle course.
        /// </summary>
        private void CalculateTargetPosition()
        {
            // virtual space:
            // 1. obstacle course start
            // 2. group previous layout
            // 3. group position
            // 4. safety margin
            // 5. obstacle space
            // 6. safety margin
            // 7. group target layout
            // 8. 3x safety margin
            // 9. obstacle course end
            var generator = Generator;
            var currentPosition = Group.Position;
            var targetLayout = TargetLayout;
            var tileSize = generator.TileSize;
            var obstacleCourseTileWidth = Mathf.FloorToInt(ObstacleCourseWidth / tileSize);
            var obstacleCourseSpace = generator.GetPreferredObstacleSpaceLength(obstacleCourseTileWidth) * tileSize;

            var currentLayoutLength = Group.Layout.Length;
            var targetLayoutLength = targetLayout.Length;

            // add 3x tileSize for extra safety margin between obstacle course limit and target layout
            ObstacleCourseLength = obstacleCourseSpace + currentLayoutLength + targetLayoutLength + tileSize * 3;
            // add 2x tileSize for extra safety margin between target layout and obstacle course
            TargetPosition = currentPosition + obstacleCourseSpace + targetLayoutLength + tileSize * 2;

            // Debug.Log("Group Position: " + currentPosition + "\n" +
            //           "Obstacle Course Space: " + obstacleCourseSpace + "\n" +
            //           "Target Layout Length: " + targetLayoutLength);
        }

        /// <summary>
        /// Calculates the group anchor for each candidate after their reactions have played out
        /// </summary>
        private void CalculateTargetAnchors()
        {
            var raceTrack = Group.RaceTrack;
            var layout = TargetLayout;
            var groupAnchorMap = GroupAnchorMap;
            var targetPosition = TargetPosition;

            var rows = layout.Rows;

            raceTrack.MainGenerator.CreateParts(targetPosition);
            var anchors = layout.CalculateAnchors(targetPosition);

            for (var y = 0; y < rows.Length; y++)
            {
                var row = rows[y];
                var candidates = row.Candidates;
                for (var x = 0; x < candidates.Length; x++)
                {
                    var candidate = candidates[x];
                    if (candidate == null) continue;
                    groupAnchorMap[candidate] = anchors[y][x];
                }
            }
        }

        private void CalculateReactions()
        {
            var slotMap = SlotMap;
            var groupAnchorMap = GroupAnchorMap;
            var agreementMap = AgreementMap;
            var agreementStatesMap = AgreementStatesMap;
            var reactionMap = ReactionMap;

            foreach (var candidate in Group.Candidates)
            {
                var slot = slotMap.TryGetValue(candidate, out var s) ? s : new CandidateSlot(-1, -1);
                var slotDelta = slot - candidate.Slot;
                var groupAnchor = groupAnchorMap.TryGetValue(candidate, out var ga) ? ga : default;
                var agreement = agreementMap.TryGetValue(candidate, out var a) ? a : 0;
                var agreementState = agreementStatesMap.TryGetValue(candidate, out var ags) ? ags : default;
                var previousAgreementScore = candidate.AgreementScore;
                var wasAlive = candidate.IsAlive;
                var reaction = new ReactionData(slot, groupAnchor, slotDelta, agreement, agreementState,
                    previousAgreementScore, wasAlive);
                reactionMap[candidate] = reaction;
            }
        }

        public void AdjustTargetPosition(float targetPosition)
        {
            var delta = targetPosition - TargetPosition;
            foreach (var key in GroupAnchorMap.Keys.ToList())
            {
                var anchor = GroupAnchorMap[key];
                anchor.y += delta;
                GroupAnchorMap[key] = anchor;
            }

            foreach (var entry in ReactionMap)
            {
                var anchor = entry.Value.GroupAnchor;
                anchor.y += delta;
                entry.Value.GroupAnchor = anchor;
            }

            TargetPosition = targetPosition;
        }

        #endregion

        public static GroupReactionResult Calculate(CandidateGroup group,
            Question[] questions, QuestionAnswer answer, IObstacleCourseGenerator generator)
        {
            var result = new GroupReactionResult(group, generator);
            result.CalculcateAgreements(questions, answer);
            result.CalculateMaxAgreement();
            result.CalculateTargetSubgroups();
            result.CalculateTargetLayout();
            result.CalculateTargetPosition();
            result.CalculateTargetAnchors();
            result.CalculateReactions();

            /*
            Debug.Log(string.Join("\n",
                result.AgreementStatesMap.Select(e => e.Key.fullName + ": " + e.Value.AgreementScore)));
                */
            return result;
        }

        public class CandidateSlotMap : Dictionary<RaceCandidate, CandidateSlot>
        {
        }

        public class CandidateVectorMap : Dictionary<RaceCandidate, RaceTrackVector>
        {
        }

        public class CandidateAgreementMap : Dictionary<RaceCandidate, int>
        {
        }

        public class CandidateReactionMap : Dictionary<RaceCandidate, ReactionData>
        {
        }

        public class CandidateAgreementStateMap : Dictionary<RaceCandidate, AgreementState>
        {
        }
    }
}