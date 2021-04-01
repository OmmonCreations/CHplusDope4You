using UnityEngine;

namespace DopeElections.Races
{
    public class PoolObstacleController : RaceObstacleController<PoolObstacle>
    {
        [SerializeField] private MeshFilter _meshFilter = null;

        private Mesh _mesh = null;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            var obstacle = Obstacle;
            var course = obstacle.Course;
            var tileSize = course.Configuration.TileSize;
            var size = obstacle.Size;

            var mesh = new Mesh();
            PoolMesh.Generate(mesh, size, tileSize);
            _meshFilter.sharedMesh = mesh;
            _mesh = mesh;
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            if(_mesh) Destroy(_mesh);
        }

        public override void PlayAppearAnimation()
        {
            
        }
    }
}