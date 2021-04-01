using System;

namespace DopeElections.Request
{
    [Serializable]
    public class RegisterRequest
    {
        public string firstName;

        public string lastName;

        public string email;

        public string username;

        public string password;

        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}