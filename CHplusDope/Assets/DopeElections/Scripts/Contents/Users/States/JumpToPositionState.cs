using DopeElections.PoliticalCharacters;
using UnityEngine;

namespace DopeElections.Users
{
    public class JumpToPositionState : PoliticalCharacterState
    {
        private Transform Transform { get; }
        
        private Vector3 Position { get; }
        private Quaternion Rotation { get; }
        private Vector3 Scale { get; }

        private float Height { get; }
        private float AnimationTime { get; }

        private AnimationCurve ArcCurve { get; }

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Vector3 _startScale;

        private float _t;

        public JumpToPositionState(PoliticalCharacterController character, Vector3 position, Quaternion rotation,
            Vector3 scale, float height, float time, AnimationCurve arcCurve) : base(character)
        {
            Transform = character.transform;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Height = height;
            AnimationTime = time;
            ArcCurve = arcCurve;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startPosition = Transform.localPosition;
            _startRotation = Transform.localRotation;
            _startScale = Transform.localScale;
            
            Controller.PlayJumpAnimation(AnimationTime);
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = Mathf.Clamp01(_t);
            var height = ArcCurve.Evaluate(progress) * Height;
            Transform.localPosition = Vector3.Lerp(_startPosition, Position + new Vector3(0, height), progress);
            Transform.localRotation = Quaternion.Lerp(_startRotation, Rotation, progress);
            Transform.localScale = Vector3.Lerp(_startScale, Scale, progress);
            if(_t>=1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Transform.localPosition = Position;
            Transform.localRotation = Rotation;
            Transform.localScale = Scale;
        }
    }
}