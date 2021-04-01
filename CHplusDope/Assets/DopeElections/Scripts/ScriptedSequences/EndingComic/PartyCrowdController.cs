using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Candidates;
using UnityEngine;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public class PartyCrowdController : MonoBehaviour
    {
        [SerializeField] private CandidateController _candidatePrefab = null;
        [SerializeField] private Transform[] _focusPoints = null;
        [SerializeField] private MeshCollider _groundCollider = null;
        [SerializeField] private PartyCandidateAnimations _animations = null;
        [SerializeField] private float _randomRotation = 20;
        [SerializeField] private float _candidateSize = 2;
        [SerializeField] private float _minJumpHeight = 4;
        [SerializeField] private Transform[] _candidateAnchors = null;

        private MeshCollider _tempCollider;
        
        private readonly List<PartyCandidateController> _controllers = new List<PartyCandidateController>();

        public int Count => _candidateAnchors.Length;

        public void Initialize()
        {
            CreateControllers();
        }

        public void PlaceCandidates(Candidate[] pool)
        {
            if (pool.Length == 0)
            {
                Debug.LogError("Cannot place candidates as there are none.");
                return;
            }

            if (_controllers.Count == 0) return;

            var candidates = pool.OrderBy(c => Random.value).Take(_controllers.Count).ToList();
            if (candidates.Count < _controllers.Count)
            {
                var availableCandidates = candidates.ToList();
                while (candidates.Count < _controllers.Count)
                {
                    candidates.AddRange(availableCandidates.Take(_controllers.Count - candidates.Count));
                }
            }

            for (var i = 0; i < _controllers.Count; i++)
            {
                var controller = _controllers[i];
                var candidate = candidates[i];
                controller.Candidate = candidate;
            }
        }

        private void CreateControllers()
        {
            _tempCollider = Instantiate(_groundCollider);
            _tempCollider.gameObject.hideFlags = HideFlags.HideAndDontSave;
            _controllers.AddRange(_candidateAnchors.Select(CreateController));
            Destroy(_tempCollider.gameObject);
        }

        private PartyCandidateController CreateController(Transform t)
        {
            var referencePosition = t.position;
            var isAboveGround = _tempCollider.Raycast(new Ray(referencePosition, Vector3.down), out var hit, 1000);
            if (isAboveGround) t.position = hit.point;

            var focusPoint = _focusPoints.OrderBy(p => (p.position - referencePosition).sqrMagnitude).First();

            var forward = (focusPoint.position - t.position);
            forward.y = 0;
            forward.Normalize();
            var randomRotation = Quaternion.Euler(0, Random.Range(-_randomRotation, _randomRotation), 0);

            t.rotation = Quaternion.LookRotation(randomRotation * forward, Vector3.up);
            t.localScale = Vector3.one * _candidateSize;

            var instance = t.gameObject.AddComponent<PartyCandidateController>();
            var instanceObject = Instantiate(_candidatePrefab.gameObject, t, false);
            var candidateController = instanceObject.GetComponent<CandidateController>();
            candidateController.Initialize();
            instanceObject.SetActive(true);
            instance.Initialize(_animations, candidateController);
            instance.JumpHeight = isAboveGround
                ? Mathf.Max(_minJumpHeight, hit.distance)
                : 0;

            return instance;
        }
    }
}