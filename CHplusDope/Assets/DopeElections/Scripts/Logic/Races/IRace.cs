using Localizator;
using UnityEngine;

namespace DopeElections.Races
{
    public interface IRace
    {
        /// <summary>
        /// Max score candidates can have without the current questions
        /// </summary>
        int BaseMaxScore { get; }
        
        /// <summary>
        /// Max score candidates can have in the current race
        /// </summary>
        int MaxScore { get; }
        
        /// <summary>
        /// Icon which represents this race in white on transparent background
        /// </summary>
        Sprite IconWhite { get; }
        
        /// <summary>
        /// Icon which represents this race in user color and white outline on transparent background
        /// </summary>
        Sprite IconOutline { get; }
        
        /// <summary>
        /// Overlay label for this race
        /// </summary>
        LocalizationKey Label { get; }
        
        /// <summary>
        /// Match type description
        /// </summary>
        LocalizationKey MatchType { get; }
        
        /// <summary>
        /// All the candidates in the current race
        /// </summary>
        RaceCandidate[] Candidates { get; }
        
        /// <summary>
        /// The current winners of the race
        /// </summary>
        RaceCandidate[] Winners { get; }
        
        void ApplyInitialState();
        void ApplyFinishedState();
        void UpdateWinners();
        
        /// <summary>
        /// The normalized progress in the current race where 0 is the initial state and 1 is the finished state
        /// </summary>
        float Progress { get; }
        /// <summary>
        /// The normalized progress step size
        /// </summary>
        float ProgressStepSize { get; }
    }
}