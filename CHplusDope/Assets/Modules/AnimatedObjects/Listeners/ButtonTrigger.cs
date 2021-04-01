using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AnimatedObjects
{
    public class ButtonTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField] private UnityEvent _onPointerDown = null;
        [SerializeField] private UnityEvent _onPointerUp = null;
        [SerializeField] private UnityEvent _onClick = null;
        [SerializeField] private UnityEvent _onClickBlocked = null;

        private Button _button;
        private bool _inside;

        public UnityEvent OnPointerDown => _onPointerDown;
        public UnityEvent OnPointerUp => _onPointerUp;
        public UnityEvent OnClick => _onClick;
        public UnityEvent OnClickBlocked => _onClickBlocked;

        private void OnEnable()
        {
            _button = GetComponent<Button>();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _inside = true;
            OnPointerDown.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            OnPointerUp.Invoke();
            if (_inside) OnPointerClick();
            _inside = false;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _inside = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            StartCoroutine(PointerExitFix());
        }

        private void OnPointerClick()
        {
            if (_button && !_button.interactable)
            {
                OnClickBlocked.Invoke();
                return;
            }

            OnClick.Invoke();
        }

        private IEnumerator PointerExitFix()
        {
            yield return new WaitForEndOfFrame();
            _inside = false;
        }
    }
}