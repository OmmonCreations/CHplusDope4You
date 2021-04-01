using StateMachines;
using UnityEngine;

namespace Tooltips
{
    public sealed class CanvasTooltip : TooltipController
    {
        private ICanvasTargetable _canvasTarget;
        
        public ICanvasTargetable CanvasTarget=> _canvasTarget;

        public override ITargetable Target
        {
            get => _canvasTarget;
            set => _canvasTarget = value as ICanvasTargetable;
        }

        protected override void Start()
        {
            base.Start();
            GoToTargetPosition();
        }

        private void Update()
        {
            GoToTargetPosition();
        }

        private void GoToTargetPosition()
        {
            SetPosition(CanvasTarget.GetPosition(TooltipsLayer,Vector2.one));
        }
    }
}