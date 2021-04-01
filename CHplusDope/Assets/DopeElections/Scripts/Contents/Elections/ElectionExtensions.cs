using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.ElectionLists;
using DopeElections.Users;
using RuntimeAssetImporter;

namespace DopeElections.Elections
{
    public static class ElectionExtensions
    {
        private static readonly int[] EmptyIdArray = new int[0];

        private static AssetPack Assets => DopeElectionsApp.Instance.Assets;

        public static CustomElectionList CreateList(this Election election, string name)
        {
            var result = CreateUserList(election);
            result.name = name;
            var assets = Assets;
            assets.PutAsset(result);
            var map = assets.GetAsset<CustomElectionListMap>(election.Key);
            if (map != null) map.Add(result.id);
            return result;
        }

        public static bool RemoveList(this Election election, CustomElectionList list)
        {
            var assets = Assets;
            assets.RemoveAsset(list);
            var map = assets.GetAsset<CustomElectionListMap>(election.Key);
            if (map != null) map.Remove(list.id);
            return true;
        }

        /// <summary>
        /// Returns all elections that have the same election date and run in the same canton
        /// </summary>
        public static IEnumerable<Election> GetParallelElections(this Election election, Canton canton,
            Constituency constituency)
        {
            var electionsOnSameDate = DopeElectionsApp.Instance.Assets.GetAssets<Election>(
                e => e.electionDate == election.electionDate
            );
            var constituencies = DopeElectionsApp.Instance.Assets.GetAssets<Constituency>();
            var validConstituencies = constituencies.Where(c =>
                c.cantonId == canton.id && (c.type == Constituency.Type.Canton || c == constituency)).ToList();
            return electionsOnSameDate.Where(e => validConstituencies.Any(c => c.id == e.constituencyId));
        }

        public static IEnumerable<ElectionList> GetLists(this Election election)
        {
            var assets = DopeElectionsApp.Instance.Assets;
            var map = assets.GetAsset<ElectionListMap>(election.Key);
            var customMap = assets.GetAsset<CustomElectionListMap>(election.Key);

            var officialListIds = map != null ? map.ListIds : EmptyIdArray;
            var customListIds = customMap != null ? customMap.ListIds : EmptyIdArray;
            var allLists = assets.GetAssets<ElectionList>()
                .Where(l => officialListIds.Contains(l.id))
                .Concat(assets.GetAssets<CustomElectionList>()
                    .Where(l => customListIds.Contains(l.id))
                );
            return allLists.Where(l => officialListIds.Contains(l.id)).ToArray();
        }

        public static IEnumerable<ElectionList> GetOfficialLists(this Election election)
        {
            var assets = DopeElectionsApp.Instance.Assets;
            var map = assets.GetAsset<ElectionListMap>(election.Key);
            if (map == null) return new ElectionList[0];

            var listIds = map.ListIds;
            var allLists = assets.GetAssets<ElectionList>();
            return allLists.Where(l => listIds.Contains(l.id)).OrderBy(l => l.number).ToArray();
        }

        public static IEnumerable<CustomElectionList> GetUserLists(this Election election)
        {
            var assets = DopeElectionsApp.Instance.Assets;
            var map = assets.GetAsset<CustomElectionListMap>(election.Key);
            if (map == null)
            {
                map = new CustomElectionListMap(election.id);
                assets.PutAsset(map);
            }

            var listIds = map.ListIds;
            var allLists = assets.GetAssets<CustomElectionList>();
            return allLists.Where(l => listIds.Contains(l.id)).ToArray();
        }

        private static CustomElectionList CreateUserList(Election election)
        {
            if (election == null) return null;
            var existingLists = DopeElectionsApp.Instance.Assets.GetAssets<CustomElectionList>();
            var result = new CustomElectionList()
            {
                id = existingLists.Length + 1,
                name = "",
                adapted = true,
                candidates = CreateCandidateEntries(election.seats)
            };
            return result;
        }

        private static ElectionList.CandidateEntry[] CreateCandidateEntries(int length)
        {
            var result = new ElectionList.CandidateEntry[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = new ElectionList.CandidateEntry()
                {
                    pos = i + 1
                };
            }

            return result;
        }

        public static IEnumerable<Candidate> GetCandidates(this Election election)
        {
            // select all available lists
            return DopeElectionsApp.Instance.Assets.GetAssets<Candidate>(
                c => election.candidates.Any(id => id == c.id)
            );
        }

        public static Constituency GetConstituency(this Election election)
        {
            return DopeElectionsApp.Instance.Assets.GetAsset<Constituency>(election.id);
        }

        public static bool IsAvailableTo(this Election election, ActiveUser user)
        {
            var assets = DopeElectionsApp.Instance.Assets;
            var electionConstituency = assets.GetAsset<Constituency>(election.constituencyId);
            if (electionConstituency == null) return false;
            
            var electionCanton = assets.GetAsset<Canton>(electionConstituency.cantonId);
            if (electionCanton == null || electionCanton.id != user.CantonId) return false;

            return electionConstituency.type == Constituency.Type.Canton ||
                   electionConstituency.id == user.ConstituencyId;
        }
    }
}