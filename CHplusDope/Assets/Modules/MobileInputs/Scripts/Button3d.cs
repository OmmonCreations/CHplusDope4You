using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MobileInputs
{
    public class Button3d : MonoBehaviour, IPointerDownListener, IPointerUpListener, ITapListener
    {
        [SerializeField] private bool _interactable = true;
        [SerializeField] private UnityEvent _onClick = null;
        [SerializeField] private UnityEvent _onPointerDown = null;
        [SerializeField] private UnityEvent _onPointerUp = null;

        public UnityEvent onClick => _onClick;
        public UnityEvent onPointerDown => _onPointerDown;
        public UnityEvent onPointerUp => _onPointerUp;

        public bool interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        public void OnTap(InputAction.CallbackContext context)
        {
            if (!interactable) return;
            _onClick.Invoke();
        }

        public void OnPointerDown(InputAction.CallbackContext context)
        {
            if (!interactable) return;
            _onPointerDown.Invoke();
        }

        public void OnPointerUp(InputAction.CallbackContext context)
        {
            if (!interactable) return;
            _onPointerUp.Invoke();
        }
    }
}