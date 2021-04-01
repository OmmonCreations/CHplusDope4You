using System;

namespace DopeElections.Answer
{
    [Serializable]
    public class QuestionAxis
    {
        public int axis;

        public int value;

        public int questionId;

        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
    }
}