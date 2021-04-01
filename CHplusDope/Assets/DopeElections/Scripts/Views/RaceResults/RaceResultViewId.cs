using Essentials;

namespace DopeElections.RaceResults
{
    public static class RaceResultViewId
    {
        private const string RaceNamespace = "race_result";

        public static readonly NamespacedKey Celebration =
            new NamespacedKey(RaceNamespace, "celebration");

        public static readonly NamespacedKey Candidate =
            new NamespacedKey(RaceNamespace, "candidate");

        public static readonly NamespacedKey MarathonReview =
            new NamespacedKey(RaceNamespace, "marathon_review");

        public static readonly NamespacedKey MarathonSmartSpider =
            new NamespacedKey(RaceNamespace, "marathon_smart_spider");

        public static readonly NamespacedKey MarathonQuestionResult =
            new NamespacedKey(RaceNamespace, "marathon_question_result");
    }
}