using System;
using RuntimeAssetImporter;

namespace DopeElections.Answer
{
    [Serializable]
    public class Canton : IAsset
    {
        public int id;

        public string name;

        public string image;

        public string backgroundImage;

        public string coatOfArms;

        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }
        public int Key => id;

    }
}