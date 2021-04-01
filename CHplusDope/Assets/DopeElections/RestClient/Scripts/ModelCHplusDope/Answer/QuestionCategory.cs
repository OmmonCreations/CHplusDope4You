using System;

namespace DopeElections.Answer
{
    [Serializable]
    public class QuestionCategory
    {
        public int id;
        public string name;
        /// <summary>
        /// URL to category icon
        /// </summary>
        public string image;
        /// <summary>
        /// CSS style definition
        /// Example: linear-gradient(20deg, #793972, #784973);
        /// Can be null.
        /// </summary>
        public string background;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }
    }
}