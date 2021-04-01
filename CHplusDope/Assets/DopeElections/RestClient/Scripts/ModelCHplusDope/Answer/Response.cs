using System;
using RuntimeAssetImporter;

namespace DopeElections.Answer
{
    [Serializable]
    public class Response : IAsset
    {
        public int candidateId;

        public int questionId;

        public int value;

        public string comment;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }

        public int Key => candidateId * 100 + questionId;
    }
}