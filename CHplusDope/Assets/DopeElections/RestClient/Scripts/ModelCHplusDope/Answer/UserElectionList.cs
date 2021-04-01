using System;

namespace DopeElections.Answer
{    
    [Serializable]

    public class UserElectionList
    {
        public int id;

        public string name;

        public bool adapted;

        public int[] candidates;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}