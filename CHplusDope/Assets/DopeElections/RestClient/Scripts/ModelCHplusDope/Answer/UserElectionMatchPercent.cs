using System;

namespace DopeElections.Answer
{
    [Serializable]

    public class UserElectionMatchPercent
    {
        public int candidateId;
        
        public float value;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
        
    }
}