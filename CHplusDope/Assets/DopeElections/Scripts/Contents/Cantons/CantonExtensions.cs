using System.Linq;
using DopeElections.Answer;

namespace DopeElections.Cantons
{
    public static class CantonExtensions
    {
        public static Constituency[] GetMunicipalities(this Canton canton)
        {
            return DopeElectionsApp.Instance.Assets
                .GetAssets<Constituency>(c => c.cantonId == canton.id && c.type == Constituency.Type.Municipal)
                .ToArray();
        }
    }
}