using System.Collections.Generic;
using System.Linq;
using DopeElections.Races;
using DopeElections.Races.RaceTracks;
using Navigation;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace DopeElections.ObstacleCourses
{
    public class RaceObstacleCourseController : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private float _timestamp = 0;

        private int[,] _occupiedMap;
#endif

        private new Transform transform { get; set; }

        public RaceController RaceController { get; private set; }
        public RaceObstacleCourse Course { get; private set; }
        public RaceObstacleController[] ObstacleControllers { get; private set; }
        public RaceTrackVector Position { get; private set; }

        private float ViewLength { get; set; }
        private float EndPosition { get; set; }

        public void Initialize(RaceController raceController, RaceObstacleCourse course, RaceTrackVector position)
        {
            RaceController = raceController;
            transform = base.transform;
            Course = course;
            Position = position;
            ObstacleControllers = CreateObstacles(course.Obstacles);
            raceController.Resetted += OnRaceResetted;

            ViewLength = raceController.CameraController.ViewLength;
            EndPosition = Position.y + course.Size.y * course.Configuration.TileSize;

            raceController.CameraController.PositionChanged += OnCameraPositionChanged;
        }

        private void OnDestroy()
        {
            if (RaceController)
            {
                RaceController.Resetted -= OnRaceResetted;
                RaceController.CameraController.PositionChanged -= OnCameraPositionChanged;
            }
        }

        public RaceTrackVector GetRaceTrackVector(Vector2 gridPoint)
        {
            return Course.GetRaceTrackVector(gridPoint, Position.y);
        }

        private void OnCameraPositionChanged(float position)
        {
            if (position - ViewLength > EndPosition)
            {
                Remove();
            }
        }

        private void OnRaceResetted()
        {
            Remove();
        }

        private RaceObstacleController[] CreateObstacles(IEnumerable<RaceObstacle> obstacles)
        {
            return obstacles.Select(CreateObstacle).Where(c => c != null).ToArray();
        }

        private RaceObstacleController CreateObstacle(RaceObstacle obstacle)
        {
            var type = obstacle.Type;
            var prefab = type.Prefab;
            if (!prefab || prefab == null)
            {
                Debug.LogWarning("Obstacle type " + type.name + " has no prefab assigned.");
                return null;
            }

            var instanceObject = Instantiate(prefab.gameObject, transform, false);
            var instance = instanceObject.GetComponent<RaceObstacleController>();
            instance.Initialize(RaceController, obstacle);
            return instance;
        }

        public void PlayAppearAnimation()
        {
            if (ObstacleControllers != null)
            {
                foreach (var c in ObstacleControllers)
                {
                    c.PlayAppearAnimation();
                }
            }
        }

        public void StopObstacles()
        {
            foreach (var o in ObstacleControllers)
            {
                o.Stop();
            }
        }

        public void Remove(float delay = 0)
        {
            if (ObstacleControllers != null)
            {
                foreach (var c in ObstacleControllers) c.Remove(delay);
            }

            Destroy(gameObject, delay);
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (Application.isPlaying) UpdateOccupantMap();
        }

        private void UpdateOccupantMap()
        {
            if (Course == null || Course.NavigationMesh == null) return;
            var size = Course.Configuration.Size;
            if (_occupiedMap == null || _occupiedMap.GetLength(1) != size.x || _occupiedMap.GetLength(0) != size.y)
            {
                _occupiedMap = new int[size.y, size.x];
            }

            var navigationMesh = Course.NavigationMesh;
            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    var position = new Vector2Int(x, y);
                    var timestamp = _timestamp;
                    _occupiedMap[y, x] = navigationMesh.Tiles[y, x].GetOccupants(position, timestamp).Count();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Course == null || Course.NavigationMesh == null) return;
            UpdateOccupantMap();
            var tiles = Course.NavigationMesh.Tiles;
            var tileSize = Course.NavigationMesh.TileSize;
            var transform = this.transform;
            var occupiedMap = _occupiedMap;
            for (var y = 0; y < tiles.GetLength(0); y++)
            {
                for (var x = 0; x < tiles.GetLength(1); x++)
                {
                    var pA = new Vector3(x * tileSize, 0, y * tileSize);
                    var pB = new Vector3((x + 1) * tileSize, 0, y * tileSize);
                    var pC = new Vector3(x * tileSize, 0, (y + 1) * tileSize);
                    var pD = new Vector3((x + 1) * tileSize, 0, (y + 1) * tileSize);
                    var a = transform.TransformPoint(pA);
                    var b = transform.TransformPoint(pB);
                    var c = transform.TransformPoint(pC);
                    var d = transform.TransformPoint(pD);
                    Handles.color = Color.black;
                    Handles.DrawAAPolyLine(1, a, b, d, c, a);
                    var occupants = occupiedMap != null ? occupiedMap[y, x] : 0;
                    if (occupants > 0)
                    {
                        var color = Color.Lerp(Color.blue, Color.red, occupants / 10f);
                        Handles.color = color;
                        Handles.DrawAAConvexPolygon(a, b, d, c);
                        continue;
                    }

                    if (tiles[y, x] is EmptyTileContent) continue;
                    {
                        var color = tiles[y, x].Color;
                        color.a = 0.2f;
                        Handles.color = color;
                        Handles.DrawAAConvexPolygon(a, b, d, c);
                    }
                }
            }
        }
#endif
    }
}