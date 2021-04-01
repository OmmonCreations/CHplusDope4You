using UnityEngine;

namespace DopeElections
{
    public class PickAnswerDisplayController : AnswerDisplayController
    {
        [SerializeField] private SingleAnswerEntryController[] _answers = null;
        
        protected override void OnUpdateState()
        {
            base.OnUpdateState();
            foreach (var answer in _answers)
            {
                answer.UserChecked = UserAnswer!=null && UserAnswer.answer == answer.Value;
                answer.CandidateChecked = CandidateAnswer != null && CandidateAnswer.value == answer.Value;
            }
        }

        protected override void OnApplyAccentColor(Color color)
        {
            base.OnApplyAccentColor(color);
            foreach (var a in _answers) a.AccentColor = color;
        }
    }
}