namespace MobileInputs
{
    public interface IDropTarget : IInteractable
    {
        bool AllowDrop(IInteractable interactable);
    }
}