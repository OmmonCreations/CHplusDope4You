using UnityEngine.InputSystem;

namespace MobileInputs
{
    public interface IPointerUpListener : IInteractable
    {
        void OnPointerUp(InputAction.CallbackContext context);
    }
}