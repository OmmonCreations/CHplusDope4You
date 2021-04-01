using DopeElections.Answer;
using SortableCollections;

namespace DopeElections.ElectionLists
{
    public class ElectionListCollection : SortableCollection<ElectionList>
    {
        public Election Election { get; }
        
        public ElectionListCollection(Election election, ElectionList[] entries) : base(entries)
        {
            Election = election;
        }
    }
}