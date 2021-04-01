using UnityEngine;

namespace Effects
{
    public class SphericalShake : Shake
    {
        public SphericalShake(float strength, float duration, Vector3 influence) : base(strength, duration, influence)
        {
        }

        public override void Run()
        {
            if (Cooldown <= 0 && Time > 0)
            {
                Target = (Random.insideUnitSphere) * Strength;
                Strength *= 0.8f;
                Cooldown = 0.05f;
            }

            if (Time > 0)
            {
                Time -= UnityEngine.Time.deltaTime;
            }
            else
            {
                Target = Vector3.zero;
                IsCompleted = true;
            }

            if (Cooldown > 0) Cooldown -= UnityEngine.Time.deltaTime;

            Position = Vector3.MoveTowards(Position, Target, UnityEngine.Time.deltaTime * Strength * 50);
        }
    }
}