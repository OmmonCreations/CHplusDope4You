using System;

namespace DopeElections.Request
{
    [Serializable]

    public class Token
    {
        public string token;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
    

}