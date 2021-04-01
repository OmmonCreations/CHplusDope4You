using DopeElections.Answer;
using QuestionAnswer = DopeElections.Answer.QuestionAnswer;

namespace DopeElections.Races
{
    public class QuestionRace
    {
        public QuestionMarathon Marathon { get; }
        public Question Question { get; }
        public int Index { get; }
        
        public QuestionAnswer Answer { get; set; }
        
        public QuestionRace(QuestionMarathon marathon, Question question, int index)
        {
            Marathon = marathon;
            Question = question;
            Index = index;
        }
    }
}