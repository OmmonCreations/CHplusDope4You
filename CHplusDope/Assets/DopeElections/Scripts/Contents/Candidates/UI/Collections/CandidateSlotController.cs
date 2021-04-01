using DopeElections.Answer;
using DopeElections.Sounds;
using FMODSoundInterface;
using MobileInputs;
using StateMachines;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DopeElections.Candidates
{
    public class CandidateSlotController : MonoBehaviour, IPointerDownHandler
    {
        public delegate void SlotEvent();

        public event SlotEvent CandidateChanged = delegate { };
        public event SlotEvent Tapped = delegate { };
        public event SlotEvent Dragged = delegate { };

        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private CandidateEntryController _candidateEntry = null;
        [SerializeField] private StateMachine _stateMachine = null;

        private ISlotContainer _container;
        private int _index;
        private Vector2Int _position;
        private float _slotSize;

        public StateMachine StateMachine => _stateMachine;
        internal RectTransform RectTransform => _rectTransform;

        public ISlotContainer Container => _container;

        public int Index
        {
            get => _index;
        }

        public Vector2Int Position
        {
            get => _position;
        }

        public int ColumnCount { get; set; }

        public float SlotSize
        {
            get => _slotSize;
        }

        public Candidate Candidate
        {
            get => _candidateEntry.Candidate;
            set
            {
                _candidateEntry.Candidate = value;
                CandidateChanged();
            }
        }

        public bool DragHorizontal { get; set; }
        public bool DragVertical { get; set; }

        public void Initialize(ISlotContainer container, int index)
        {
            _container = container;
            _candidateEntry.Candidate = null;
            RectTransform.anchorMin = Vector2.up;
            RectTransform.anchorMax = Vector2.up;
            _index = index;
            ApplySlotSize(container.SlotSize);
            ApplyInteractionSystem(container.InteractionSystem);
        }

        private void Update()
        {
            StateMachine.Run();
        }

        private void OnDestroy()
        {
            if (_container != null)
            {
                ReleaseHooks(_container.InteractionSystem);
            }
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        private void ApplySlotSize(float size)
        {
            _slotSize = size;
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }

        private void ApplyPosition(Vector2Int position)
        {
            _position = position;
            _index = position.y * ColumnCount + position.x;
        }

        private void ApplyInteractionSystem(InteractionSystem interactionSystem)
        {
            interactionSystem.OnPointerMove += OnPointerMove;
            interactionSystem.OnPointerUp += OnPointerUp;
        }

        private void ReleaseHooks(InteractionSystem interactionSystem)
        {
            interactionSystem.OnPointerMove -= OnPointerMove;
            interactionSystem.OnPointerUp -= OnPointerUp;
        }

        public void SetPosition(Vector2Int position, float delay = 0)
        {
            SetPosition(position, GetSlotVector(position, SlotSize), delay);
        }

        public void SetPosition(Vector2Int slotPosition, Vector2 position, float delay = 0)
        {
            var state = new MoveToPositionState(this, position, delay);
            StateMachine.State = state;
            ApplyPosition(slotPosition);
        }

        public void SetPositionImmediately(Vector2Int slotPosition)
        {
            SetPositionImmediately(slotPosition, GetSlotVector(slotPosition, SlotSize));
        }

        public void SetPositionImmediately(Vector2Int slotPosition, Vector2 position)
        {
            RectTransform.anchoredPosition = position;
            ApplyPosition(slotPosition);
        }

        public void UpdateFrame() => _candidateEntry.UpdateFrame();

        public static Vector2 GetSlotVector(Vector2Int position, float slotSize)
        {
            return new Vector2(position.x + 0.5f, -(position.y + 0.5f)) * slotSize;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StateMachine.State = new PrepareDragState(this, eventData.position);
        }

        private void OnPointerMove(IInteractable interactable, InputAction.CallbackContext context)
        {
            var dragState = StateMachine.State as PrepareDragState;
            if (dragState != null) dragState.OnPointerMove(context);
        }

        private void OnPointerUp(IInteractable interactable, InputAction.CallbackContext context)
        {
            var dragState = StateMachine.State as PrepareDragState;
            if (dragState != null) dragState.OnPointerUp(context);
        }

        internal void TriggerTapped()
        {
            Tapped();
            SoundController.Play(Sound.UI.Button.Tap);
        }

        internal void TriggerDragged()
        {
            Dragged();
        }
    }
}