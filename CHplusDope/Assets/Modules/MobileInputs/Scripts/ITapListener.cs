using UnityEngine.InputSystem;

namespace MobileInputs
{
    public interface ITapListener : IInteractable
    {
        void OnTap(InputAction.CallbackContext context);
    }
}