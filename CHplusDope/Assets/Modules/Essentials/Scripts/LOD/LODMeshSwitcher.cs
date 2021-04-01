using System;
using UnityEngine;

namespace Essentials.LOD
{
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteAlways]
    public class LODMeshSwitcher : LODSwitcher
    {
        [SerializeField][HideInInspector] private MeshFilter _filter = null;

        private void OnEnable()
        {
            _filter = GetComponent<MeshFilter>();
        }

        protected override void ApplyMesh(Mesh mesh)
        {
            if (_filter) _filter.sharedMesh = mesh;
        }
    }
}