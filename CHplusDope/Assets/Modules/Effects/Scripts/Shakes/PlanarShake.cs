using UnityEngine;

namespace Effects
{
    public class PlanarShake : Shake
    {
        public PlanarShake(float strength, float duration, Vector3 influence) : base(strength, duration, influence)
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

            var target = new Vector3(Target.x, Target.y, 0);
            Position = Vector3.MoveTowards(Position, target, UnityEngine.Time.deltaTime * Strength * 50);
        }
    }
}