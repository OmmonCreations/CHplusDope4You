using System;

namespace DopeElections.Answer
{
    [Serializable]
    public class RegisterAnswer
    {
        public string message;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}