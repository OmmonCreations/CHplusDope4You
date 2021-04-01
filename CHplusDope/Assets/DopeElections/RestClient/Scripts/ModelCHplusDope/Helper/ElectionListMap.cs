using System.Collections.Generic;
using RuntimeAssetImporter;

namespace DopeElections
{
    public class ElectionListMap : IAsset
    {
        public int Key => ElectionId;

        public int ElectionId { get; }
        private readonly List<int> _listIds = new List<int>();

        public int[] ListIds => _listIds.ToArray();

        public ElectionListMap(int electionId, IEnumerable<int> listIds = null)
        {
            ElectionId = electionId;
            if (listIds != null) _listIds.AddRange(listIds);
        }

        public void Add(int listId) => _listIds.Add(listId);

        public void Remove(int listId) => _listIds.Remove(listId);
    }
}