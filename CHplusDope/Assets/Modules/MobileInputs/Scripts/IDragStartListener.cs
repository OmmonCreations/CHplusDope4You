using UnityEngine.InputSystem;

namespace MobileInputs
{
    public interface IDragStartListener : IInteractable
    {
        void OnDragStart(InputAction.CallbackContext context);
    }
}