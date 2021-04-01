using System;
using RuntimeAssetImporter;

namespace DopeElections.Answer
{
    [Serializable]
    public class Constituency : IAsset
    {       
        public int Key => id;
        
        public int id;

        public string name;

        public string coatOfArms;

        public string type;

        public int cantonId;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }

        public static class Type
        {
            public const string Municipal = "municipal";
            public const string Canton = "canton";
        }
    }
}