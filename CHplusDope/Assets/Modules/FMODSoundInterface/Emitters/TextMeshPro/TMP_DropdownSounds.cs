using System;
using TMPro;
using UnityEngine;

namespace FMODSoundInterface.TextMeshPro
{
    public class TMP_DropdownSounds : AbstractSoundEmitter<TMP_Dropdown>
    {
        public string expandEvent = TextMeshProSound.Dropdown.Expand;
        public string collapseEvent = TextMeshProSound.Dropdown.Collapse;
        public string selectEvent = TextMeshProSound.Dropdown.Select;
        
        [Header("Auto-Link (optional)")]
        [SerializeField] private TMP_Dropdown _source = null;

        private EventListener _listener;
        
        protected override TMP_Dropdown Source => _source;

        public override void HookEvents(TMP_Dropdown source)
        {
            source.onValueChanged.AddListener(Select);
            var listener = source.template.gameObject.AddComponent<EventListener>();
            listener.Initialize();
            listener.Expanded += Expand;
            listener.Collapsed += Collapse;
            _listener = listener;
        }

        public override void ReleaseHooks(TMP_Dropdown source)
        {
            source.onValueChanged.RemoveListener(Select);
            if(_listener) _listener.Remove();
        }

        private void Select(int value) => Select();

        public void Expand()
        {
            SoundController.Play(expandEvent);
        }

        public void Collapse()
        {
            SoundController.Play(collapseEvent);
        }

        public void Select()
        {
            SoundController.Play(selectEvent);
        }

        private class EventListener : MonoBehaviour
        {
            public delegate void DropdownEvent();

            public event DropdownEvent Expanded = delegate { };
            public event DropdownEvent Collapsed = delegate { };

            private bool _initialized = false;
            
            private void OnEnable()
            {
                if (!_initialized) return;
                Expanded();
            }

            private void OnDisable()
            {
                if (!_initialized) return;
                Collapsed();
            }

            public void Initialize()
            {
                _initialized = true;
            }

            public void Remove()
            {
                Destroy(this);
            }
        }
    }
}