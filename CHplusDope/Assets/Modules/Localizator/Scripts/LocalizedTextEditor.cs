using Essentials;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Localizator
{
    public sealed class LocalizedTextEditor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        internal LocalizedText target { get; set; }
        private bool _isEditing = false;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Keyboard.current.ctrlKey.isPressed)
            {
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerDownHandler);
                return;
            }
            
            StartEditing();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerUpHandler);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerClickHandler);
        }

        void StartEditing()
        {
            if (!target) target = GetComponent<LocalizedText>();
            if (_isEditing || target.localization == null) return;
            _isEditing = true;
            var targetObject = target.gameObject;
            var targetTransform = targetObject.GetComponent<RectTransform>();
            var targetColor = target.textComponent.color;
            
            var instance = Instantiate(target.gameObject, targetTransform.GetComponentInParent<Canvas>().transform, false);
            instance.name = instance.name.Replace("(Clone)", "(Editor)");
            var instanceTransform = (RectTransform) instance.transform;
            Destroy(instance.GetComponent<LocalizedTextEditor>());
            Destroy(instance.GetComponent<LocalizedText>());
            var textInstance = instance.GetComponent<TMP_Text>();
            textInstance.color = Color.white;
            instance.AddComponent<LayoutElement>().ignoreLayout = true;
            instanceTransform.Overlay(targetTransform);
            
            var keyTextInstance = Instantiate(instance);
            keyTextInstance.gameObject.name = "LocalizationKey";

            var keyTextTransform = keyTextInstance.GetComponent<RectTransform>();
            keyTextTransform.SetParent(instanceTransform);
            keyTextTransform.anchorMax = new Vector2(1, 0.5f);
            keyTextTransform.anchorMin = new Vector2(1, 0.5f);
            keyTextTransform.pivot = new Vector2(0, 0.5f);
            keyTextTransform.anchoredPosition = Vector2.zero;
            var keyText = keyTextInstance.GetComponent<TMP_Text>();
            keyText.color = new Color(0.8f, 0.8f, 0.8f);
            keyText.text = "#"+target.key;
            keyText.horizontalAlignment = HorizontalAlignmentOptions.Left;
            keyText.verticalAlignment = VerticalAlignmentOptions.Middle;
            keyText.fontSize = 12;
            
            var inputField = instance.AddComponent<TMP_InputField>();
            inputField.textComponent = textInstance;
            inputField.textViewport = instanceTransform;
            inputField.text = target.text;
            inputField.lineType = TMP_InputField.LineType.MultiLineSubmit;
            var listener = new UnityAction<string>(value =>
            {
                Destroy(instance);
                EndEditing(value);
                target.textComponent.color = targetColor;
            });
            inputField.onDeselect.AddListener(listener);
            inputField.onSubmit.AddListener(listener);
            inputField.Select();

            target.textComponent.color = Color.clear;
        }

        void EndEditing(string value)
        {
            if (!_isEditing) return;
            _isEditing = false;
            var localization = target.localization;
            localization.SetString(target.key, value);
            if (target)
            {
                target.UpdateLabels();
                target.gameObject.SetActive(true);
            }
        }
    }
}