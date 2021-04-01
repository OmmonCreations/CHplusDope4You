using UnityEngine.InputSystem;

namespace MobileInputs
{
    public interface IPointerDownListener : IInteractable
    {
        void OnPointerDown(InputAction.CallbackContext context);
    }
}