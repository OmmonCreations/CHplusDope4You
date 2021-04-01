using System;

namespace DopeElections.Request
{
    [Serializable]

    public class MatchRequest
    {
        public string token;
        
        public string type;
        
        public int rank;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}