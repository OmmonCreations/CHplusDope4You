using DopeElections.Answer;
using UnityEngine;

namespace DopeElections
{
    public abstract class AnswerDisplayController : MonoBehaviour
    {
        private Color _accentColor;
        
        public QuestionAnswer UserAnswer { get; set; }
        public Response CandidateAnswer { get; set; }
        
        public Color AccentColor
        {
            get => _accentColor;
            set => ApplyAccentColor(value);
        }
        public void UpdateState()
        {
            OnUpdateState();
        }

        private void ApplyAccentColor(Color color)
        {
            _accentColor = color;
            OnApplyAccentColor(color);
        }

        protected virtual void OnApplyAccentColor(Color color)
        {
            
        }

        protected virtual void OnUpdateState()
        {
            
        }
    }
}