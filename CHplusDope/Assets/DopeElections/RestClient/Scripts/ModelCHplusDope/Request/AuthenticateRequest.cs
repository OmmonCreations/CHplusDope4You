using System;

namespace DopeElections.Request
{        
    [Serializable]
    
    public class AuthenticateRequest
    {
        public string email;
        
        public string password;

        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}