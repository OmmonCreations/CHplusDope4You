using UnityEngine;

namespace Essentials.LOD
{
    public abstract class LODSwitcher : MonoBehaviour
    {
        [SerializeField] private int _level = 0;
        [SerializeField] private Mesh[] _levels = new Mesh[0];

        public int level
        {
            get => _level;
            set => ApplyLevel(value);
        }

        private void ApplyLevel(int level)
        {
            _level = level;
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            if (level < 0 || level >= _levels.Length) return;
            var mesh = _levels[level];
            if(mesh) ApplyMesh(mesh);
        }

        protected abstract void ApplyMesh(Mesh mesh);

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateMesh();
        }
#endif
    }
}