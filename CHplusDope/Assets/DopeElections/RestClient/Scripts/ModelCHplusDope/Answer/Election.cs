using System;
using RuntimeAssetImporter;

namespace DopeElections.Answer
{
    [Serializable]

    public class Election : IAsset
    {
        public int id;

        public int constituencyId;

        public int electionId;
            
        public int seats;
        
        public string name;
        
        public int repetitions;
        
        public string logo;
                
        public string electionDate;
                
        public string label;
                  
        public string type;
                
        public string updatedAt;

        [NonSerialized] public long electionTimestamp;

        [NonSerialized] public int[] candidates;
            
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }

        public int Key => id;
    }
}