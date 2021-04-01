using DopeElections.Users;
using Essentials;
using UnityEngine;
using Views;

namespace DopeElections.Accounts
{
    public abstract class AccountView : DopeElectionsView, IView<ActiveUser>
    {
        public AccountViewsContainer Views { get; private set; }
        public AccountSceneController EditorController { get; private set; }
        public ActiveUser User { get; private set; }
        protected PlayerController PlayerController => EditorController.PlayerController;

        protected override void OnInitialize(ViewsContainer viewsContainer)
        {
            Views = viewsContainer as AccountViewsContainer;
            EditorController = Views != null ? Views.EditorController : null;
        }

        public void Open(ActiveUser user)
        {
            User = user;
            base.Open();
        }

        protected void AttachPlayerToAnchor(Transform anchor)
        {
            AttachTransformToAnchor(PlayerController.transform, anchor);
        }

        protected static void AttachTransformToAnchor(Transform transform, Transform anchor)
        {
            transform.SetParent(anchor, false);
            transform.Reset();
        }

        protected static void MoveTransformToAnchor(Transform transform, Transform anchor)
        {
            transform.position = anchor.position;
            transform.rotation = anchor.rotation;
            transform.localScale = anchor.localScale;
        }
    }
}