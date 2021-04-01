using Essentials;
using Newtonsoft.Json.Linq;

namespace DopeElections.Users
{
    public class CandidateUserData
    {
        public int CandidateId { get; }
        public NamespacedKey Hat { get; set; }
        public NamespacedKey Mount { get; set; }

        public CandidateUserData(int candidateId)
        {
            CandidateId = candidateId;
        }

        public JObject Serialize()
        {
            return new JObject()
            {
                ["id"] = CandidateId,
                ["hat"] = Hat.Serialize(),
                ["mount"] = Mount.Serialize()
            };
        }

        public static bool TryParse(JToken json, out CandidateUserData userData)
        {
            return TryParse(json as JObject, out userData);
        }

        public static bool TryParse(JObject json, out CandidateUserData userData)
        {
            if (json == null)
            {
                userData = null;
                return false;
            }

            var candidateId = json["id"] != null ? (int) json["id"] : 0;
            if (candidateId == default)
            {
                userData = null;
                return false;
            }

            NamespacedKey.TryParse(json["hat"], out var hat);
            NamespacedKey.TryParse(json["mount"], out var mount);

            userData = new CandidateUserData(candidateId)
            {
                Hat = hat,
                Mount = mount
            };

            return true;
        }
    }
}