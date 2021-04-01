using System;

namespace DopeElections.Request
{
    [Serializable]

    public class ResponseRequest
    {
        public string token;
        public int value;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}