#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using UnityEngine;

namespace Navigation
{
    public class TileGridPathVisualizer : MonoBehaviour
    {
        private TileGridNavMesh NavigationMesh { get; set; }
        public CompiledPath Path { get; set; }

        public void Initialize(TileGridNavMesh navigationMesh)
        {
            NavigationMesh = navigationMesh;
        }

        private Vector3 GetWorldPoint(Vector2Int tile)
        {
            var tileSize = NavigationMesh.TileSize;
            return new Vector3(tile.x * tileSize, 0, tile.y * tileSize);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (NavigationMesh == null || Path == null || Path.Actions.Length == 0) return;
            var transform = this.transform;
            Handles.DrawAAPolyLine(4f, Path.Actions.Select(a => a.From).Append(Path.Actions.Last().To)
                .Select(p => transform.TransformPoint(GetWorldPoint(p)))
                .ToArray()
            );
        }
#endif
    }
}