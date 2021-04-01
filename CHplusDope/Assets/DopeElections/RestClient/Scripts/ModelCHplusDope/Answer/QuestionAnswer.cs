using Newtonsoft.Json.Linq;

namespace DopeElections.Answer
{
    public class QuestionAnswer
    {
        public int questionId;
        
        public int answer;

        public QuestionAnswer(int questionId, int answer)
        {
            this.questionId = questionId;
            this.answer = answer;
        }
        
        public JObject Serialize()
        {
            return new JObject()
            {
                ["q"] = questionId,
                ["a"] = answer
            };
        }

        public static bool TryParse(JToken json, out QuestionAnswer questionAnswer)
        {
            return TryParse(json as JObject, out questionAnswer);
        }

        public static bool TryParse(JObject json, out QuestionAnswer questionAnswer)
        {
            if (json == null)
            {
                questionAnswer = null;
                return false;
            }

            int questionId = json["q"] != null ? (int) json["q"] : 0;
            int answer = json["a"] != null ? (int) json["a"] : 0;
            
            if (questionId == 0)
            {
                questionAnswer = null;
                return false;
            }
            
            questionAnswer = new QuestionAnswer(questionId, answer);
            
            return true;

        }
    }

}