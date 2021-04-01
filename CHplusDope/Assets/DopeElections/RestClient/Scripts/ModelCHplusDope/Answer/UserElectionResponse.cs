using System;

namespace DopeElections.Answer
{
    [Serializable]

    public class UserElectionResponse
    {
        public string questionId;
        
        public int value;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}