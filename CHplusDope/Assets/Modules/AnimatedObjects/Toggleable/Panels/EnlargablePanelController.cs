using StateMachines;
using UnityEngine;

namespace AnimatedObjects
{
    public class EnlargablePanelController : PoppablePanelController
    {
        [SerializeField] private float _enlargedSize = 1.2f;
        
        public float EnlargedSize
        {
            get => _enlargedSize;
            set => _enlargedSize = value;
        }

        protected override Vector3 FromSize { get; } = Vector3.one;
        protected override Vector3 ToSize => Vector3.one * _enlargedSize;
        protected override bool ControlGameObjectActiveState { get; } = false;

        public TransitionState Pop(float delay)
        {
            Show(delay);
            return Hide(AppearTime + delay);
        }
    }
}