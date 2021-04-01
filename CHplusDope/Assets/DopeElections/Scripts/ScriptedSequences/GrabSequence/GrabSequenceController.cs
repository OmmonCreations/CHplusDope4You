using System;
using CameraSystems;
using DopeElections.Sounds;
using DopeElections.Users;
using FMODSoundInterface;
using Localizator;
using MobileInputs;
using PopupInfos;
using UnityEngine;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public class GrabSequenceController : MonoBehaviour
    {
        [SerializeField] private Transform _sequenceEnvironment = null;
        [SerializeField] private CameraSystem _cameraSystem = null;
        [SerializeField] private GrabSequencePlayerController _playerController = null;
        [SerializeField] private GrabArmController _grabArmController = null;
        [SerializeField] private PopupInfoLayer _popupInfoLayer = null;
        [SerializeField] private LocalizationScope _localizationScope = null;
        [SerializeField] private InteractionSystem _interactionSystem = null;

        public CameraSystem CameraSystem => _cameraSystem;
        public PopupInfoLayer PopupInfoLayer => _popupInfoLayer;
        public ILocalization Localization => _localizationScope.Localization;
        public InteractionSystem InteractionSystem => _interactionSystem;
        
        private Action Callback { get; set; }
        
        public void Initialize()
        {
            _grabArmController.Grabbed += OnPlayerGrabbed;
            _grabArmController.Disappeared += OnGrabArmDisappeared;
            _grabArmController.Initialize(this);
            PopupInfoLayer.Initialize(CameraSystem);
            
            _sequenceEnvironment.gameObject.SetActive(false);
        }

        public void Launch(Action callback)
        {
            Callback = callback;
            
            InteractionSystem.IgnoreUI = true;

            _sequenceEnvironment.gameObject.SetActive(true);
            
            var grabArmController = _grabArmController;
            var playerController = _playerController;
            var playerTransform = playerController.PlayerController.transform;
            playerTransform.SetParent(playerController.PlayerAnchor, false);
            playerTransform.localPosition = Vector3.zero;
            playerTransform.localRotation = Quaternion.identity;
            playerTransform.localScale = Vector3.one;
            
            playerController.PlayerController.Face = PlayerFaceId.Missing;
            playerController.Float();
            
            grabArmController.gameObject.SetActive(false);
            grabArmController.Launch(playerController);
        }

        private void OnPlayerGrabbed(bool success)
        {
            if (!success) return;
            if (Callback != null) Callback();
        }

        private void OnGrabArmDisappeared()
        {
            _sequenceEnvironment.gameObject.SetActive(false);
        }
    }
}