using System;
using Essentials;

namespace DopeElections.Accounts
{
    public static class AccountViewId 
    {
        public static NamespacedKey Login { get; } = new NamespacedKey("account","login");
        public static NamespacedKey Settings { get; } = new NamespacedKey("account","settings");
        public static NamespacedKey Profile { get; } = new NamespacedKey("account","profile");
        public static NamespacedKey FaceSelection { get; } = new NamespacedKey("account","face_selection");
        public static NamespacedKey LocationSelection { get; } = new NamespacedKey("account","location_selection");
        public static NamespacedKey ComicSequence { get; } = new NamespacedKey("account","comic_sequence");
        [Obsolete]
        public static NamespacedKey LegacyComicSequence { get; } = new NamespacedKey("account","legacy_comic_sequence");
    }
}