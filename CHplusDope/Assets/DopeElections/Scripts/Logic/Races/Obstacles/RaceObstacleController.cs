using UnityEngine;

namespace DopeElections.Races
{
    public abstract class RaceObstacleController : MonoBehaviour
    {
        public RaceController RaceController { get; private set; }
        public RaceObstacle Obstacle { get; private set; }

        public virtual void Initialize(RaceController raceController, RaceObstacle obstacle)
        {
            RaceController = raceController;
            Obstacle = obstacle;
            ApplyPosition(obstacle.Position);
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        protected void OnDestroy()
        {
            OnDestroyed();
        }

        protected virtual void OnDestroyed()
        {
            
        }

        public void Stop()
        {
            OnStopped();
        }

        protected virtual void OnStopped()
        {
            
        }

        public abstract void PlayAppearAnimation();

        public void Remove(float delay = 0)
        {
            Destroy(gameObject, delay);
        }

        private void ApplyPosition(Vector2Int tilePosition)
        {
            var transform = this.transform;
            var courseConfiguration = Obstacle.Course.Configuration;
            var tileSize = courseConfiguration.TileSize;
            var position = new Vector3(tilePosition.x * tileSize, 0, tilePosition.y * tileSize);
            transform.localPosition = position;
        }
    }

    public abstract class RaceObstacleController<T> : RaceObstacleController where T : RaceObstacle
    {
        public new T Obstacle { get; private set; }

        public sealed override void Initialize(RaceController raceController, RaceObstacle obstacle)
        {
            Obstacle = obstacle as T;
            base.Initialize(raceController, obstacle);
        }
    }
}