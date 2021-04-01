using System.Collections.Generic;
using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    public class CandidateReactionContext
    {
        public RaceObstacleCourse Course { get; }
        
        /// <summary>
        /// The candidate controller
        /// </summary>
        public RaceCandidateController Controller { get; }

        /// <summary>
        /// The candidate
        /// </summary>
        public RaceCandidate Candidate { get; }

        /// <summary>
        /// The reaction data of the candidate
        /// </summary>
        public ReactionData ReactionData { get; }

        /// <summary>
        /// The tile position of the candidate before the group reaction has played out
        /// </summary>
        public Vector2Int From { get; }

        /// <summary>
        /// The tile position of the candidate after the group reaction has played out
        /// </summary>
        public Vector2Int To { get; }

        /// <summary>
        /// The currently calculated paths this candidate takes in this order
        /// </summary>
        public List<RawPath> Paths { get; }

        /// <summary>
        /// The tile position of the candidate after clearing all of the currently provided paths
        /// </summary>
        public Vector2Int Position { get; set; }

        /// <summary>
        /// How much this candidate's total travel time differs from the rest of the group where a positive value
        /// implies wait time and a negative value implies boost time
        /// </summary>
        public float Timestamp { get; set; }

        /// <summary>
        /// How much this candidate's total travel time differs from the rest of the group where a positive value
        /// implies wait time and a negative value implies boost time
        /// </summary>
        public float TimeDifference { get; set; }

        public CandidateReactionContext(RaceObstacleCourse course, RaceCandidateController candidate, 
            ReactionData reaction, Vector2Int from, Vector2Int to, float timeDifference)
        {
            Course = course;
            Controller = candidate;
            Candidate = candidate.Candidate;
            ReactionData = reaction;
            Paths = new List<RawPath>();
            From = from;
            To = to;
            Position = from;
            TimeDifference = timeDifference;
        }
    }
}