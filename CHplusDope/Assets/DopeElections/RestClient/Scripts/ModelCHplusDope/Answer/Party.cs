using System;
using RuntimeAssetImporter;

namespace DopeElections.Answer
{
    [Serializable]
    public class Party : IAsset
    {
        public int id;

        public string logo;

        public string abbr;
        
        public string name;

        public string color;
        
        public string onColor;
        
        public string onWhite;
        
        public string onBlack;
        
        public int Key => id;
    }
}