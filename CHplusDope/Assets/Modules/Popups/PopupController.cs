using System;
using AnimatedObjects;
using UnityEngine;

namespace Popups
{
    public abstract class PopupController : MonoBehaviour
    {
        [SerializeField] private ToggleableObjectController _animationController = null;
        
        public Popup Popup { get; private set; }

        internal virtual void Initialize(Popup popup)
        {
            Popup = popup;
            popup.Closed += OnClose;
            OnInitialize();

            if(_animationController) _animationController.HideImmediate();
        }

        internal void Open()
        {
            if(_animationController) _animationController.Show();
            OnOpen();
        }

        protected virtual void OnOpen()
        {
            
        }

        internal void OnClose()
        {
            OnClosed();
        }

        protected abstract void OnInitialize();

        public void Remove()
        {
            Destroy(gameObject);
            OnRemove();
        }

        protected virtual void OnClosed()
        {
            if (_animationController) _animationController.Hide().Then(Remove);
            else Remove();
        }
        
        protected virtual void OnRemove(){}
    }

    public abstract class PopupController<T> : PopupController where T : Popup
    {
        public new T Popup { get; private set; }

        internal sealed override void Initialize(Popup popup)
        {
            Popup = popup as T;
            base.Initialize(popup);
        }
    }
}