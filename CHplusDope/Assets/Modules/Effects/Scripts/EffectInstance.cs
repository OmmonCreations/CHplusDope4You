using Essentials;
using StateMachines;
using UnityEngine;

namespace Effects
{
    public abstract class EffectInstance : MonoBehaviour
    {
        public delegate void EffectEvent();

        public event EffectEvent Finished = delegate { };

        public virtual int MaxInstances { get; } = -1;
        
        public NamespacedKey TypeId => NamespacedKey.TryParse(_typeId, out var id) ? id : default;

        [SerializeField] private string _typeId = null;

        private Transform _referenceTransform;
        
        private bool _finished = false;
        
        public ISpatialTargetable EffectTarget { get; set; }

        public Vector3 Offset { get; set; } = Vector3.zero;

        public virtual Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public virtual Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }
        
        public Transform ReferenceTransform
        {
            get => _referenceTransform;
            set => ApplyReferenceTransform(value);
        }
        
        private Transform Transform { get; set; }
        
        protected virtual void Awake()
        {
            Transform = transform;
        }

        protected virtual  void Start()
        {
            
        }

        protected virtual void Update()
        {
            if (EffectTarget == null) return;

            Transform.position = EffectTarget.Position + Offset;
            Transform.rotation = EffectTarget.Rotation;
        }

        protected virtual void OnDestroy()
        {
            if(!_finished) Finish();
        }

        public virtual void Play(EffectData data = null)
        {
            OnPlay();
        }

        protected virtual void OnPlay()
        {
            
        }
        
        public abstract void Pause();

        public abstract void Resume();

        public void Remove()
        {
            Finish();
            OnRemove();
            Destroy();
        }

        protected virtual void OnRemove()
        {
            
        }

        protected virtual void Destroy()
        {
            Destroy(gameObject);
        }

        private void Finish()
        {
            if (_finished) return;
            _finished = true;
            OnFinish();
            Finished();
        }

        protected virtual void OnFinish()
        {
            
        }

        private void ApplyReferenceTransform(Transform t)
        {
            _referenceTransform = t;
            OnReferenceTransformChanged(t);
        }

        protected virtual void OnReferenceTransformChanged(Transform t)
        {
            
        }
    }
    
    public abstract class EffectInstance<T> : EffectInstance where T : EffectData
    {
        protected T Data { get; private set; }

        public sealed override void Play(EffectData data = null)
        {
            if (data is T effectData) Data = effectData;
            base.Play(data);
        }
    }
}