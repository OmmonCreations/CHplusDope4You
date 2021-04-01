using Essentials;

namespace DopeElections.Progress
{
    public static class ProgressViewId
    {
        private const string Namespace = "progress";
        
        public static readonly NamespacedKey Leaderboard = new NamespacedKey(Namespace, "leaderboard");
        public static readonly NamespacedKey LeaderboardFilter = new NamespacedKey(Namespace, "leaderboard_filter");
        public static readonly NamespacedKey Progress = new NamespacedKey(Namespace, "progress");
        public static readonly NamespacedKey Congratulations = new NamespacedKey(Namespace, "congratulations");
        public static readonly NamespacedKey ExtraInfo = new NamespacedKey(Namespace, "extra_info");
    }
}