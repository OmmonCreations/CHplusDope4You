using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FMODSoundInterface.UnityUI
{
    public class ButtonSounds : AbstractSoundEmitter<Button>
    {
        public string tapEvent = UISound.Button.Tap;
        public string confirmEvent = UISound.Button.Confirm;
        public string cancelEvent = UISound.Button.Cancel;
        public string releaseEvent = null;
        public Type type = Type.Normal;
        
        [Header("Auto-Link (optional)")]
        [SerializeField] private Button _source = null;

        private EventListener _listener;

        protected override Button Source => _source;

        public override void HookEvents(Button source)
        {
            var listener = source.gameObject.AddComponent<EventListener>();
            listener.Pressed += Press;
            listener.Released += Release;
            _listener = listener;
        }

        public override void ReleaseHooks(Button source)
        {
            if(_listener) _listener.Remove();
        }
        
        public void Press(Type type)
        {
            switch (type)
            {
                case Type.Normal: TapNormal();
                    break;
                case Type.Confirm: Confirm();
                    break;
                case Type.Cancel: Cancel();
                    break;
            }
        }

        public void Press()
        {
            Press(type);
        }
        
        public void TapNormal()
        {
            SoundController.Play(tapEvent);
        }

        public void Confirm()
        {
            SoundController.Play(confirmEvent);
        }

        public void Cancel()
        {
            SoundController.Play(cancelEvent);
        }
        
        public void Release()
        {
            SoundController.Play(releaseEvent);
        }

        public enum Type
        {
            Normal,
            Confirm,
            Cancel
        }

        private class EventListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
        {
            public delegate void ButtonEvent();

            public event ButtonEvent Pressed = delegate { };
            public event ButtonEvent Released = delegate { };
            
            

            public void Remove()
            {
                Destroy(this);
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                Pressed();
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                Released();
            }
        }
    }
}