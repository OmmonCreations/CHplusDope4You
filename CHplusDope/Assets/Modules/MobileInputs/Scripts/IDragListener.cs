using UnityEngine.InputSystem;

namespace MobileInputs
{
    public interface IDragListener : IInteractable
    {
        void OnDrag(InputAction.CallbackContext context);
    }
}