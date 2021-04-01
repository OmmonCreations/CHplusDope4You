using StateMachines;
using UnityEngine;

namespace AnimatedObjects.Transforms
{
    public class EnlargableTransformController : PoppableTransformController
    {
        [SerializeField] private float _normalSize = 1f;
        [SerializeField] private float _enlargedSize = 1.2f;

        public float NormalSize
        {
            get => _normalSize;
            set => _normalSize = value;
        }

        public float EnlargedSize
        {
            get => _enlargedSize;
            set => _enlargedSize = value;
        }

        protected override Vector3 FromSize => Vector3.one * NormalSize;
        protected override Vector3 ToSize => Vector3.one * EnlargedSize;
        protected override bool ControlGameObjectActiveState { get; } = false;

        public TransitionState Pop(float delay)
        {
            Show(delay);
            return Hide(AppearTime + delay);
        }
    }
}