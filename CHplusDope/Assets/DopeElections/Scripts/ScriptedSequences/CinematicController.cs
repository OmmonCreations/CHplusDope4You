using System.Linq;
using UnityEngine;

namespace DopeElections.ScriptedSequences
{
    public abstract class CinematicController : ScriptedSequenceController
    {
        [SerializeField] private CinematicControls _controls = null;

        public CinematicControls Controls => _controls;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controls.Initialize(this);
        }

        protected override void OnBeforePlay(ScriptedSequenceState[] parts)
        {
            base.OnBeforePlay(parts);
            Controls.AnimationController.HideImmediate();
            foreach (var p in parts.OfType<ICinematicState>())
            {
                Controls.Bind(p);
            }
        }

        public virtual void Skip()
        {
            if (!(StateMachine.State is ScriptedSequenceState state)) return;
            state.Skip();
        }
    }
}