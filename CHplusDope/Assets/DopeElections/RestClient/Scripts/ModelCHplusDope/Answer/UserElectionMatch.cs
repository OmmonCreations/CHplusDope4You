using System;

namespace DopeElections.Answer
{
    [Serializable]

    public class UserElectionMatch
    {
        public int candidateId;
        
        public string type;
        
        public int rank;

        public float percent;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}