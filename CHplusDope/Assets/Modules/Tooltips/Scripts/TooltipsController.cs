using System;
using System.Collections.Generic;
using System.Linq;
using CameraSystems;
using Localizator;
using StateMachines;
using UnityEngine;

namespace Tooltips
{
    public class TooltipsController : MonoBehaviour, ITooltipSource
    {
        [SerializeField] private SpatialTooltip _spatialTooltipPrefab = null;
        [SerializeField] private CanvasTooltip _canvasTooltipPrefab = null;
        [SerializeField] private RectTransform _spatialTooltipsArea = null;
        [SerializeField] private RectTransform _canvasTooltipsArea = null;
        [SerializeField] private CameraSystem _cameraSystem = null;

        [Header("Generic Contents")] 
        [SerializeField] private TooltipContentController[] _templates = null;

        internal CameraSystem CameraSystem => _cameraSystem;
        
        public ILocalization Localization { get; set; } = new DefaultLocalization();

        internal TooltipContentController[] Templates => _templates;
        
        protected TooltipController[] Tooltips
        {
            get
            {
                return _canvasTooltipsArea.GetComponentsInChildren<TooltipController>()
                    .Concat(_spatialTooltipsArea.GetComponentsInChildren<TooltipController>()).ToArray();
            }
        }

        private void Start()
        {
            _canvasTooltipsArea.pivot = Vector2.up;
            _spatialTooltipsArea.pivot = Vector2.up;
        }

        public TooltipController ShowTooltip(ISpatialTargetable target, Tooltip tooltip)
        {
            return ShowTooltip(target, _spatialTooltipPrefab, _spatialTooltipsArea, tooltip);
        }

        public TooltipController ShowTooltip(ICanvasTargetable target, Tooltip tooltip)
        {
            return ShowTooltip(target, _canvasTooltipPrefab, _canvasTooltipsArea, tooltip);
        }
        
        private TooltipController ShowTooltip(ITargetable target, TooltipController prefab, RectTransform layer, Tooltip tooltip)
        {
            var instanceObject = Instantiate(prefab.gameObject, layer, false);
            var instance = instanceObject.GetComponent<TooltipController>();
            instance.Initialize(this, layer);
            instance.Target = target;
            instance.Tooltip = tooltip;
            instanceObject.SetActive(true);
            return instance;
        }
    }
}