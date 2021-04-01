using Newtonsoft.Json.Linq;

namespace DopeElections.Questions
{
    public readonly struct QuestionCategoryWinner
    {
        public int CategoryId { get; }
        public int Rank { get; }
        public int CandidateId { get; }

        public QuestionCategoryWinner(int categoryId, int rank, int candidateId)
        {
            CategoryId = categoryId;
            Rank = rank;
            CandidateId = candidateId;
        }

        public JObject Serialize()
        {
            return new JObject
            {
                ["cat"] = CategoryId,
                ["ran"] = Rank,
                ["can"] = CandidateId
            };
        }

        public static bool TryParse(JToken json, out QuestionCategoryWinner result)
        {
            return TryParse(json as JObject, out result);
        }

        public static bool TryParse(JObject json, out QuestionCategoryWinner result)
        {
            if (json == null)
            {
                result = default;
                return false;
            }

            var categoryId = json["cat"] != null ? (int) json["cat"] : 0;
            var rank = json["ran"] != null ? (int) json["ran"] : 0;
            var candidateId = json["can"] != null ? (int) json["can"] : 0;

            if (categoryId == 0 || candidateId == 0)
            {
                result = default;
                return false;
            }

            result = new QuestionCategoryWinner(categoryId, rank, candidateId);
            return true;
        }
        
        public bool Equals(QuestionCategoryWinner other)
        {
            return CategoryId == other.CategoryId && CandidateId == other.CandidateId;
        }

        public override bool Equals(object obj)
        {
            return obj is QuestionCategoryWinner other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CategoryId * 397) ^ CandidateId;
            }
        }
    }
}