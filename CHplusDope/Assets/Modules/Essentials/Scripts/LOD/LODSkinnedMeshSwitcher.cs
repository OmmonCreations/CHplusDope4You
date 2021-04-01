using UnityEngine;

namespace Essentials.LOD
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    [ExecuteAlways]
    public class LODSkinnedMeshSwitcher : LODSwitcher
    {
        [SerializeField][HideInInspector] private SkinnedMeshRenderer _renderer = null;

        private void OnEnable()
        {
            _renderer = GetComponent<SkinnedMeshRenderer>();
        }

        protected override void ApplyMesh(Mesh mesh)
        {
            _renderer.sharedMesh = mesh;
        }
    }
}