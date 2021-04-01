using UnityEngine;

namespace DopeElections.Prefabs
{
    public class CandidateSelectionEnvironmentAnim : MonoBehaviour
    {
        [SerializeField] private Animator candidateAnimator = null;
        private int danceInt;
        private Renderer rend;

        void Start()
        {
            rend = GetComponentInChildren<Renderer>();
        
            RandomDance();  
        
        }

    
        void Update()
        {
        
        }

        private void RandomDance()
        {
            int randomDance = Random.Range(0, 5);
            candidateAnimator.SetInteger("danceInt", randomDance);
        }

    
    }
}
