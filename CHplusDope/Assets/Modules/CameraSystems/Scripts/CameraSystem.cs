using BlackMasks;
using Essentials;
using StateMachines;
using UnityEngine;

namespace CameraSystems
{
    public class CameraSystem : MonoBehaviour
    {
        private static CameraSystem _instance = null;

        public static CameraSystem Instance
        {
            get
            {
                if (_instance) return _instance;
                return null;
            }
        }

        [Header("Referenzen")] [SerializeField]
        private Transform _anchor = null;

        [SerializeField] private Transform _pivot = null;
        [SerializeField] private Transform _cameraTransform = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private BlackMask _fadeToBlackOverlay = null;
        [SerializeField] private Camera _camera = null;

        private Quaternion _rotation = Quaternion.identity;

        public Transform Anchor
        {
            get { return _anchor; }
        }

        public Transform Pivot
        {
            get { return _pivot; }
        }

        public Transform CameraTransform
        {
            get { return _cameraTransform; }
        }

        public float FadeToBlack
        {
            get => _fadeToBlackOverlay.Alpha;
            set => _fadeToBlackOverlay.Alpha = value;
        }

        public virtual float FieldOfView
        {
            get { return Camera.fieldOfView; }
            set { Camera.fieldOfView = value; }
        }

        public StateMachine StateMachine => _stateMachine;

        public Camera Camera => _camera;

        public CameraTransformation CurrentTransform
        {
            get =>
                new CameraTransformation
                {
                    position = _anchor.localPosition,
                    eulerAngles = new Vector3(
                        _pivot.localEulerAngles.x,
                        _anchor.localEulerAngles.y,
                        _cameraTransform.localEulerAngles.z),
                    distance = -_cameraTransform.localPosition.z,
                    fov = FieldOfView
                };
            set
            {
                _anchor.localPosition = value.position;
                _anchor.localRotation = Quaternion.Euler(0, value.eulerAngles.y, 0);
                _pivot.localEulerAngles = new Vector3(value.eulerAngles.x, 0, 0);
                _cameraTransform.localPosition = new Vector3(0, 0, -value.distance);
                _cameraTransform.localEulerAngles = new Vector3(0, 0, value.eulerAngles.z);
                FieldOfView = value.fov;
            }
        }

        protected virtual void Awake()
        {
            if (_instance)
            {
                return;
            }

            _instance = this;
        }

        protected virtual void Update()
        {
            StateMachine.Run();
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        protected virtual void OnEnable()
        {
            if (_camera) _camera.enabled = true;
        }

        protected virtual void OnDisable()
        {
            if (_camera) _camera.enabled = false;
        }

        public ApplyCameraTransformationState Transition(CameraTransformation cameraTransformation, float time)
        {
            return Transition(cameraTransformation, time, AnimationCurve.EaseInOut(0, 0, 1, 1));
        }

        public ApplyCameraTransformationState TransitionLinear(CameraTransformation cameraTransformation, float time)
        {
            return Transition(cameraTransformation, time, AnimationCurve.Linear(0, 0, 1, 1));
        }

        public ApplyCameraTransformationState Transition(CameraTransformation cameraTransformation, float time,
            AnimationCurve animation)
        {
            var result = new ApplyCameraTransformationState(this, cameraTransformation, time, animation);
            StateMachine.State = result;
            return result;
        }

        public CameraFocusTargetState Focus(IFocusable target, float distance = -1)
        {
            var result = new CameraFocusTargetState(Instance, target, distance);
            Instance.StateMachine.State = result;
            return result;
        }

        public Vector3 GetWorldPosition(Vector2 screenPosition, LayerMask layerMask)
        {
            return GetWorldPosition(screenPosition, layerMask, out var collider);
        }

        public Vector3 GetWorldPosition(Vector2 screenPosition, LayerMask layerMask, out Collider collider)
        {
            var c = Instance.Camera;
            // ReSharper disable once PossibleLossOfFraction
            var r = c.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, layerMask))
            {
                collider = hit.collider;
                return hit.point;
            }

            var p = new Plane(Vector3.up, Vector3.zero);
            float enter;
            collider = null;
            return p.Raycast(r, out enter) ? r.GetPoint(enter) : Vector3.zero;
        }

        public Vector2 GetScreenPoint(Vector3 worldPoint)
        {
            return RectTransformUtility.WorldToScreenPoint(Camera, worldPoint);
        }

        public Vector2 GetViewportPosition(RectTransform viewport, Vector3 worldPoint)
        {
            var screenPoint = GetScreenPoint(worldPoint);
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, screenPoint, Camera, out var p)
                ? p
                : Vector2.negativeInfinity;
        }

        public static CameraSystem CreateDefault()
        {
            var defaultTransformation = new CameraTransformation()
            {
                distance = 10,
                fov = 50
            };
            var gameObject = new GameObject("CameraSystem");
            var result = gameObject.AddComponent<CameraSystem>();
            result._anchor = gameObject.transform;
            var pivot = new GameObject("Pivot").transform;
            pivot.SetParent(result._anchor, false);
            result._pivot = pivot;
            var cameraAnchor = new GameObject("CameraAnchor").transform;
            cameraAnchor.SetParent(result._pivot, false);
            result._cameraTransform = cameraAnchor;
            var camera = new GameObject("Camera");
            camera.transform.SetParent(result._cameraTransform, false);
            result._camera = camera.AddComponent<Camera>();
            result._stateMachine = gameObject.AddComponent<StateMachine>();
            result.CurrentTransform = defaultTransformation;
            return result;
        }
    }
}