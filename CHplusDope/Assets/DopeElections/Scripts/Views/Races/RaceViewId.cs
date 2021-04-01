using Essentials;

namespace DopeElections.Races
{
    public static class RaceViewId
    {
        private const string RaceNamespace = "race";
        
        public static readonly NamespacedKey CategorySelection = new NamespacedKey(RaceNamespace, "category_selection");
        public static readonly NamespacedKey Question = new NamespacedKey(RaceNamespace, "question");
        public static readonly NamespacedKey QuestionInfo = new NamespacedKey(RaceNamespace, "question_info");
        public static readonly NamespacedKey SubgroupLeaderboard = new NamespacedKey(RaceNamespace, "subgroup_leaderboard");
    }
}