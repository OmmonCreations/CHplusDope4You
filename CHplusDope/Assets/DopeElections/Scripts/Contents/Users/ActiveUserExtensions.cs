using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.ElectionLists;
using DopeElections.Elections;
using UnityEngine;

namespace DopeElections.Users
{
    public static class ActiveUserExtensions
    {
        public static ElectionList[] GetAvailableOfficialLists(this ActiveUser user)
        {
            return user.GetElection().GetLists().ToArray();
        }

        public static CustomElectionList[] GetAvailableCustomLists(this ActiveUser user)
        {
            var election = user.GetElection();
            return election != null ? election.GetUserLists().ToArray() : new CustomElectionList[0];
        }

        public static Election[] GetAvailableElections(this ActiveUser user)
        {
            var constituencies = DopeElectionsApp.Instance.Assets.GetAssets<Constituency>(c =>
                (c.cantonId == user.CantonId && c.type == Constituency.Type.Canton) ||
                c.id == user.ConstituencyId
            );
            return DopeElectionsApp.Instance.Assets.GetAssets<Election>(
                e => constituencies.Any(c => c.id == e.constituencyId)
            );
        }

        public static IEnumerable<Candidate> GetRegionalCandidates(this ActiveUser user)
        {
            var elections = user.GetAvailableElections();
            var assets = DopeElectionsApp.Instance.Assets;
            var candidates = assets.GetAssets<Candidate>();
            return elections
                .SelectMany(e => e.candidates)
                .Distinct()
                .Select(id => candidates.FirstOrDefault(c => c.id == id));
        }

        public static IEnumerable<Candidate> GetAvailableCandidates(this ActiveUser user)
        {
            var election = user.GetElection();
            if (election != null)
            {
                return election.GetCandidates();
            }

            return user.GetRegionalCandidates();
        }

        public static Canton GetCanton(this ActiveUser user)
        {
            return DopeElectionsApp.Instance.Assets.GetAsset<Canton>(user.CantonId);
        }

        public static Constituency GetConstituency(this ActiveUser user)
        {
            return DopeElectionsApp.Instance.Assets.GetAsset<Constituency>(user.ConstituencyId);
        }

        public static Election GetElection(this ActiveUser user)
        {
            var electionId = user.ElectionId;
            return DopeElectionsApp.Instance.Assets.GetAsset<Election>(electionId);
        }

        public static CustomElectionList GetActiveList(this ActiveUser user)
        {
            var listId = user.ListId;
            return DopeElectionsApp.Instance.Assets.GetAsset<CustomElectionList>(listId);
        }
    }
}