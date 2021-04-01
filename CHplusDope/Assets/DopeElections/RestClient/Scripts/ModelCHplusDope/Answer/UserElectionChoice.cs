using System;

namespace DopeElections.Answer
{
    [Serializable]

    public class UserElectionChoice
    {
        public int id;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}