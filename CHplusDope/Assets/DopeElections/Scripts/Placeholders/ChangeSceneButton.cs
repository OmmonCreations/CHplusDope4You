using AppManagement;
using Essentials;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Placeholders
{
    public class ChangeSceneButton : MonoBehaviour
    {
        [SerializeField] private string _sceneId = null;
        [SerializeField] private Button _button = null;

        private void Start()
        {
            var sceneId = NamespacedKey.TryParse(_sceneId, out var id) ? id : default;
            _button.onClick.AddListener(() => ApplicationController.LoadScene(sceneId));
        }
    }
}