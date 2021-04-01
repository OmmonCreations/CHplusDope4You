using System.Linq;
using UnityEngine;

namespace Popups
{
    public class PopupsLayer : MonoBehaviour
    {
        [SerializeField] private RectTransform _layer = null;
        [SerializeField] private PopupController[] _prefabs = null;

        public PopupController ShowPopup<T>(T popup) where T : Popup
        {
            var prefab = _prefabs.OfType<PopupController<T>>().FirstOrDefault();
            if (!prefab)
            {
                Debug.LogWarning("No popup prefab of type " + typeof(T).Name + " found in layer " + gameObject.name +
                                 "!");
                return null;
            }

            var result = CreatePrompt(prefab, popup);
            result.Open();
            return result;
        }

        private T CreatePrompt<T, T2>(T prefab, T2 popup) where T : PopupController<T2> where T2 : Popup
        {
            var instanceObject = Instantiate(prefab.gameObject, _layer, false);
            var instance = instanceObject.GetComponent<T>();
            instance.Initialize(popup);
            return instance;
        }
    }
}