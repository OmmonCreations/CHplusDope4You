using System;

namespace DopeElections.Answer
{
    [Serializable]

    public class User
    {
        public int id;

        public string username;

        public string email;
        
        public string progression;
        
        public string cantonId;

        public string constituencyId;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}