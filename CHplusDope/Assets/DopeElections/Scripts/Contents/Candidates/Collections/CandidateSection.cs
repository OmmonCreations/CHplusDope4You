using DopeElections.Answer;
using Localizator;

namespace DopeElections.Candidates
{
    public class CandidateSection
    {
        public LocalizationKey Title { get; }
        public Candidate[] Candidates { get; }

        public CandidateSection(Candidate[] candidates) : this(default, candidates)
        {
            
        }
        
        public CandidateSection(LocalizationKey title, Candidate[] candidates)
        {
            Title = title;
            Candidates = candidates;
        }
    }
}