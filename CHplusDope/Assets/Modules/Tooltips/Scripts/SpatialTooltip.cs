using StateMachines;
using UnityEngine;

namespace Tooltips
{
    public sealed class SpatialTooltip : TooltipController
    {
        private ISpatialTargetable _spatialTarget;
        private Camera _camera;
        
        public ISpatialTargetable SpatialTarget
        {
            get { return _spatialTarget; }
        }

        public override ITargetable Target
        {
            get => _spatialTarget;
            set => _spatialTarget = value as ISpatialTargetable;
        }

        protected override void Start()
        {
            base.Start();
            _camera = TooltipsController.CameraSystem.Camera;
            GoToTargetPosition();
        }

        private void Update()
        {
            if (Input.mousePresent)
            {
                GoToMousePosition();
            }
        }

        private void GoToTargetPosition()
        {
            var worldPosition = SpatialTarget.Position + new Vector3(0, SpatialTarget.Height / 2);
            var screenPosition = _camera.WorldToScreenPoint(worldPosition);
            SetPosition(screenPosition);
        }
    }
}