using MobileInputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DopeElections.LoveMobiles
{
    public class LoveMobileController : MonoBehaviour, ITapListener
    {
        public delegate void TapEvent();

        public event TapEvent Pressed = delegate { };

        [SerializeField] private Transform _meshTransform = null;

        public bool Interactable { get; set; }

        public Transform MeshTransform => _meshTransform;
        
        public void OnTap(InputAction.CallbackContext context)
        {
            if(!Interactable) return;
            Pressed();
        }
    }
}