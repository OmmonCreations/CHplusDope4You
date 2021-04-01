using System.Collections.Generic;

namespace DopeElections.ElectionLists
{
    public class CustomElectionListMap : ElectionListMap
    {
        public CustomElectionListMap(int electionId, IEnumerable<int> listIds = null) : base(electionId, listIds)
        {
            
        }
    }
}