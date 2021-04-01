using UnityEngine.InputSystem;

namespace MobileInputs
{
    public interface IPointerMoveListener : IInteractable
    {
        void OnPointerMove(InputAction.CallbackContext context);
    }
}