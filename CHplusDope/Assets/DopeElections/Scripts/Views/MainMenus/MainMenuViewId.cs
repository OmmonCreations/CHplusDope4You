using Essentials;

namespace DopeElections.MainMenus
{
    public static class MainMenuViewId
    {
        private const string MainMenuNamespace = "main";
        
        public static readonly NamespacedKey Overview = new NamespacedKey(MainMenuNamespace, "overview");
        public static readonly NamespacedKey ListsAndCandidates = new NamespacedKey(MainMenuNamespace, "lists_and_candidates");
        public static readonly NamespacedKey Team = new NamespacedKey(MainMenuNamespace, "team");
        public static readonly NamespacedKey Informations = new NamespacedKey(MainMenuNamespace, "informations");
        public static readonly NamespacedKey VoteInfos = new NamespacedKey(MainMenuNamespace, "vote_infos");
        public static readonly NamespacedKey EndingCinematic = new NamespacedKey(MainMenuNamespace, "ending_cinematic");
        public static readonly NamespacedKey EndingCredits = new NamespacedKey(MainMenuNamespace, "ending_credits");
        public static readonly NamespacedKey Final = new NamespacedKey(MainMenuNamespace, "final");
        
        // informations subviews
        public static readonly NamespacedKey HowToPlay = new NamespacedKey(MainMenuNamespace, "how_to_play");
        public static readonly NamespacedKey Credits = new NamespacedKey(MainMenuNamespace, "credits");
        
        // vote info subviews
        public static readonly NamespacedKey HowToVote = new NamespacedKey(MainMenuNamespace, "how_to_vote");
        public static readonly NamespacedKey OurSystem = new NamespacedKey(MainMenuNamespace, "our_system");
        
        public static readonly NamespacedKey LandSequence = new NamespacedKey(MainMenuNamespace, "land_sequence");
    }
}