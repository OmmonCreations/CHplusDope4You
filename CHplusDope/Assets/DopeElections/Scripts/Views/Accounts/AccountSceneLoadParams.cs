using System;
using AppManagement;
using DopeElections.Users;
using Essentials;

namespace DopeElections.Accounts
{
    public class AccountSceneLoadParams : SceneLoadParams
    {
        public ActiveUser User { get; }
        public NamespacedKey MenuId { get; }
        public Action<bool> Callback { get; }

        public AccountSceneLoadParams(NamespacedKey menuId = default, Action<bool> callback = null)
        {
            User = DopeElectionsApp.Instance.User;
            MenuId = menuId;
            Callback = callback;
        }
        
        public AccountSceneLoadParams(ActiveUser user, NamespacedKey menuId = default, Action<bool> callback = null)
        {
            User = user;
            MenuId = menuId;
            Callback = callback;
        }
    }
}