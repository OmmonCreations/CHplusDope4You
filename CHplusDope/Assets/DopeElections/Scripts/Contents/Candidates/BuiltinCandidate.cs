using UnityEngine;

namespace DopeElections.Candidates
{
    [CreateAssetMenu(fileName = "Candidate", menuName = "Dope Elections/Candidate")]
    public class BuiltinCandidate : ScriptableObject
    {
        public const string City = "Dope Town";
    
        [SerializeField] private Sprite _portrait = null;
        [SerializeField] private string _firstname = null;
        [SerializeField] private string _lastname = null;

        public Sprite Portrait => _portrait;
        public string Firstname => _firstname;
        public string Lastname => _lastname;
    }
}