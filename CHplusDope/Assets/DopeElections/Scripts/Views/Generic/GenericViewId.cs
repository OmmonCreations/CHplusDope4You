using Essentials;

namespace DopeElections
{
    public static class GenericViewId
    {
        private const string GenericNamespace = "generic";
        
        public static readonly NamespacedKey Candidate = new NamespacedKey(GenericNamespace, "candidate");
        public static readonly NamespacedKey CandidateProfile = new NamespacedKey(GenericNamespace, "candidate_profile");
        public static readonly NamespacedKey CandidateAnswers = new NamespacedKey(GenericNamespace, "candidate_answers");
        public static readonly NamespacedKey SmartSpiderInfo = new NamespacedKey(GenericNamespace, "smart_spider_info");
        public static readonly NamespacedKey RaceInfo = new NamespacedKey(GenericNamespace, "race_info");
    }
}