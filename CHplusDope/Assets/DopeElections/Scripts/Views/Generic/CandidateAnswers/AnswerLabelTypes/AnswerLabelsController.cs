using UnityEngine;

namespace DopeElections.AnswerLabelTypes
{
    public abstract class AnswerLabelsController : MonoBehaviour
    {
        public void Initialize()
        {
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }
    }
}