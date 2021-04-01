using System;

namespace DopeElections.Answer
{
    [Serializable]

    public class AuthenticationAnswer
    {
        public string token;

        public User user;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}
