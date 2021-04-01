using System;

namespace DopeElections.ObstacleCourses
{
    [Serializable]
    public class SmartSpiderAxisAssociation
    {
        public Axis axis;
        public bool aligned = true;

        public enum Axis
        {
            Any = 0,
            LiberalForeignPolitics = 1,
            LiberalEconomy = 2,
            RestrictiveFinances = 3,
            LawAndOrder = 4,
            RestrictiveMigration = 5,
            EnvironmentalProtection = 6,
            SocialState = 7,
            LiberalSociety = 8
        }
    }
}