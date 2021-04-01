using System;
using System.Collections.Generic;
using System.Linq;
using Essentials;
using UnityEngine;

namespace Effects
{
    public class EffectsController : MonoBehaviour
    {
        [Header("Effect Templates")]
        [SerializeField] private EffectLibrary[] _libraries = null;
        
        private readonly Dictionary<NamespacedKey,List<EffectInstance>> _instances = new Dictionary<NamespacedKey, List<EffectInstance>>();

        private bool _hideAll;
        
        public EffectInstance PlayEffect<T>(NamespacedKey typeId, T data) where T : EffectData
        {
            var prefab = GetPrefab<T>(typeId);
            return PlayEffect(prefab, Vector3.zero, Quaternion.identity, data);
        }

        public EffectInstance PlayEffect(NamespacedKey typeId, Vector3 position)
        {
            return PlayEffect(typeId, position, Quaternion.identity);
        }

        public EffectInstance PlayEffect<T>(NamespacedKey typeId, Vector3 position, T data) where T : EffectData
        {
            return PlayEffect(typeId, position, Quaternion.identity, data);
        }
        
        public EffectInstance PlayEffect(NamespacedKey typeId, Vector3 position, Quaternion rotation)
        {
            var prefab = GetPrefab(typeId);
            return PlayEffect(prefab, position, rotation);
        }
        
        public EffectInstance<T> PlayEffect<T>(NamespacedKey typeId, Vector3 position, Quaternion rotation,
            T data) where T : EffectData
        {
            var prefab = GetPrefab<T>(typeId);
            return PlayEffect(prefab, position, rotation, data);
        }

        public EffectInstance<T> PlayEffect<T>(NamespacedKey typeId, Transform parent,
            T data) where T : EffectData
        {
            var prefab = GetPrefab<T>(typeId);
            return PlayEffect(prefab, parent, data);
        }

        public EffectInstance<T> PlayEffect<T>(NamespacedKey typeId, Transform parent, Transform reference,
            T data) where T : EffectData
        {
            var prefab = GetPrefab<T>(typeId);
            return PlayEffect(prefab, parent, reference, data);
        }

        public CanvasEffectInstance<T> PlayEffect<T>(NamespacedKey typeId, RectTransform parent,
            T data) where T : EffectData
        {
            var prefab = GetPrefab<T>(typeId) as CanvasEffectInstance<T>;
            return PlayEffect(prefab, parent, data);
        }
        
        public T PlayEffect<T>(T prefab, Vector3 position, Quaternion rotation) where T : EffectInstance
        {
            var instanceObject = Instantiate(prefab.gameObject, position, rotation);
            var instance = instanceObject.GetComponent<T>();
            return PlayEffect(instance);
        }

        public T PlayEffect<T>(T prefab, Transform parent, bool worldPositionStays = false) where T : EffectInstance
        {
            var instanceObject = Instantiate(prefab.gameObject, parent, worldPositionStays);
            var instance = instanceObject.GetComponent<T>();
            return PlayEffect(instance);
        }

        public T PlayEffect<T>(T prefab, Transform parent, Transform reference, bool worldPositionStays = false) where T : EffectInstance
        {
            var instanceObject = Instantiate(prefab.gameObject, parent, worldPositionStays);
            var instance = instanceObject.GetComponent<T>();
            instance.ReferenceTransform = reference;
            return PlayEffect(instance);
        }
        
        public T PlayEffect<T,T2>(T prefab, Vector3 position, Quaternion rotation, T2 arguments) where T2 : EffectData where T : EffectInstance<T2>
        {
            var instanceObject = Instantiate(prefab.gameObject, position, rotation);
            var instance = instanceObject.GetComponent<T>();
            return PlayEffect(instance, arguments);
        }
        
        public T PlayEffect<T,T2>(T prefab, Transform parent, T2 arguments) where T2 : EffectData where T : EffectInstance
        {
            var instanceObject = Instantiate(prefab.gameObject, parent, false);
            var instance = instanceObject.GetComponent<T>();
            return PlayEffect(instance, arguments);
        }
        
        public T PlayEffect<T,T2>(T prefab, Transform parent, Transform referenceTransform, T2 arguments) where T2 : EffectData where T : EffectInstance
        {
            var instanceObject = Instantiate(prefab.gameObject, parent, false);
            var instance = instanceObject.GetComponent<T>();
            instance.ReferenceTransform = referenceTransform;
            return PlayEffect(instance, arguments);
        }
        
        public T PlayEffect<T,T2>(T prefab, RectTransform parent, T2 arguments) where T2 : EffectData where T : CanvasEffectInstance<T2>
        {
            var instanceObject = Instantiate(prefab.gameObject, parent, false);
            var instance = instanceObject.GetComponent<T>();
            return PlayEffect(instance, arguments);
        }
        
        private T PlayEffect<T>(T instance) where T : EffectInstance
        {
            var instanceObject = instance.gameObject;
            instance.Play();
            if (_hideAll)
            {
                instanceObject.SetActive(false);
                Destroy(instanceObject, 0.5f);
            }

            if (instance.MaxInstances > 0)
            {
                EnforceMaxInstances(instance);
            }
            return instance;
        }
        
        private T PlayEffect<T,T2>(T instance, T2 arguments) where T2 : EffectData where T : EffectInstance
        {
            var instanceObject = instance.gameObject;
            instance.Play(arguments);
            if (_hideAll)
            {
                instanceObject.SetActive(false);
                Destroy(instanceObject, 0.5f);
            }

            if (instance.MaxInstances > 0)
            {
                EnforceMaxInstances(instance);
            }
            return instance;
        }

        private void EnforceMaxInstances(EffectInstance created)
        {
            var instances = _instances;
            var existing = instances.TryGetValue(created.TypeId, out var e) ? e : null;
            if (existing == null)
            {
                existing = new List<EffectInstance>();
                instances[created.TypeId] = existing;
            }
            instances[created.TypeId].Add(created);
            var removeList = existing.GetRange(0, existing.Count - created.MaxInstances);
            foreach (var effectInstance in removeList)
            {
                effectInstance.Remove();
            }
                
            created.Finished += () => instances[created.TypeId].Remove(created);
        }

        private EffectInstance GetPrefab(NamespacedKey typeId)
        {
            var template = _libraries.Select(l => l.GetEffect(typeId)).FirstOrDefault(e => e);
            if(!template) Debug.LogError("No effect of type "+typeId+" found!");
            return template;
        }

        private EffectInstance<T> GetPrefab<T>(NamespacedKey typeId) where T : EffectData
        {
            var template = _libraries.Select(l => l.GetEffect(typeId)).FirstOrDefault(e => e);
            if(!template) Debug.LogError("No effect of type "+typeId+" found!");
            return template as EffectInstance<T>;
        }

        public void HideAll(bool hide)
        {
            _hideAll = hide;
        }
    }
}