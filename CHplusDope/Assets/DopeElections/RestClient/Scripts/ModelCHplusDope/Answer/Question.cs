using System;

namespace DopeElections.Answer
{
    [Serializable]
    public class Question
    {
        public int id;

        public string type;

        public string text;

        public bool isShort;

        public string info;

        public string pro;

        public string contr;

        public string category;
        
        public int categoryId;

        public string categoryImage;

        public string categoryBackground;

        [NonSerialized] public QuestionAxis[] axis;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}