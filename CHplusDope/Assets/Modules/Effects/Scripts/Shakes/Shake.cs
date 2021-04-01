using UnityEngine;

namespace Effects
{
    public abstract class Shake
    {       
        private Vector3 _position; //localPosition
        
        private float _cooldown = 0; //shakeCooldown
        private float _strength; //shakeStrength
        private float _time; //shakeTime
        private Vector3 _target; //shakeTarget
        private readonly Vector3 _influence; //shakeInfluence
        
        public bool IsCompleted { get; protected set; }
        
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        
        public float Duration
        {
            get { return _time; }
            set { _time = value; }
        }

        protected float Cooldown
        {
            get { return _cooldown; }
            set { _cooldown = value; }
        }

        protected float Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }

        protected float Time
        {
            get { return _time; }
            set { _time = value; }
        }

        protected Vector3 Target
        {
            get { return _target; }
            set { _target = value; }
        }

        protected Vector3 Influence
        {
            get { return _influence; }
        }

        public Shake(float strength, float duration, Vector3 influence)
        {
            _strength = strength;
            _time = duration;
            _influence = influence;
        }

        public abstract void Run();
    }
}