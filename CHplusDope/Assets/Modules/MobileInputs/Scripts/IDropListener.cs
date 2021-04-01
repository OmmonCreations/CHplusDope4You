using UnityEngine.InputSystem;

namespace MobileInputs
{
    public interface IDropListener : IInteractable
    {
        void OnDrop(InputAction.CallbackContext context, IInteractable target);
    }
}