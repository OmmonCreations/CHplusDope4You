namespace DopeElections.Races
{
    public readonly struct AgreementState
    {
        public int AgreementScore { get; }
        public float CategoryMatch { get; }

        public AgreementState(int agreementScore, float categoryMatch)
        {
            AgreementScore = agreementScore;
            CategoryMatch = categoryMatch;
        }
    }
}