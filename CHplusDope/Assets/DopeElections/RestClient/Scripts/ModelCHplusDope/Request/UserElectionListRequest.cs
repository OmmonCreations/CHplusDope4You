using System;

namespace DopeElections.Request
{
    [Serializable]

    public class UserElectionListRequest
    {
        public string token;
        
        public string name;

        public bool adapted;

        public int[] candidates;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}