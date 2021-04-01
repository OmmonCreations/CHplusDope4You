using System;
using RuntimeAssetImporter;

namespace DopeElections.Answer
{
    [Serializable]

    public class ElectionList : IAsset
    {
        public delegate void UpdateEvent(ElectionList list);

        public static event UpdateEvent ListUpdated = delegate { };
        
        public int Key => id;
        
        public int id;
        
        public int number;
        
        public string name;
        
        public CandidateEntry[] candidates;
                
        public bool adapted;
        
        public override string ToString(){
            return UnityEngine.JsonUtility.ToJson (this, true);
        }

        public static void TriggerUpdate(ElectionList list)
        {
            ListUpdated(list);
        }
        
        [Serializable]
        public class CandidateEntry
        {
            public int pos;
            public int id;
        }
    }
}

