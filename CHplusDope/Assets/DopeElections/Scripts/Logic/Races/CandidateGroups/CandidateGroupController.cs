using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.ObstacleCourses;
using DopeElections.Races.RaceTracks;
using StateMachines;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DopeElections.Races
{
    /// <summary>
    /// View representation of candidate group
    /// </summary>
    public class CandidateGroupController : MonoBehaviour
    {
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private CandidateSubgroupController _subgroupPrefab = null;
        [SerializeField] private CandidateGroupComposition _composition = null;

        public StateMachine StateMachine => _stateMachine;
        public CandidateGroupComposition Composition => _composition;

        public RaceController RaceController { get; private set; }
        public CandidateGroup Group { get; private set; }

        private float _forwardPosition;
        private readonly List<CandidateSubgroupController> _subgroups = new List<CandidateSubgroupController>();

        public IEnumerable<CandidateSubgroupController> SubgroupControllers => _subgroups;

        public void Initialize(RaceController raceController, CandidateGroup group, float startPosition)
        {
            RaceController = raceController;
            Group = group;
            HookEvents();
            OnSubgroupsChanged(group.Groups);
            OnLayoutChanged(group.Layout);
            group.Position = startPosition;
        }

        private void Update()
        {
            StateMachine.Run();
        }

        private void OnDestroy()
        {
            ReleaseHooks();
        }

        public void Unload()
        {
            ReleaseHooks();
            Group = null;
            _forwardPosition = 0;
            foreach (var subgroup in _subgroups) Destroy(subgroup.gameObject);
            _subgroups.Clear();
        }

        private void HookEvents()
        {
            var group = Group;
            if (group != null)
            {
                group.PositionChanged += OnPositionChanged;
                group.SubgroupsChanged += OnSubgroupsChanged;
                group.LayoutChanged += OnLayoutChanged;
            }
        }

        private void ReleaseHooks()
        {
            var group = Group;
            if (group != null)
            {
                group.PositionChanged -= OnPositionChanged;
                group.SubgroupsChanged -= OnSubgroupsChanged;
                group.LayoutChanged -= OnLayoutChanged;
            }
        }

        private void OnPositionChanged(float forwardPosition)
        {
            var position = RaceController.RaceTrack.GetWorldPosition(new RaceTrackVector(0.5f, forwardPosition,
                RaceTrackVector.AxisType.Percentage, RaceTrackVector.AxisType.Distance));
            RaceController.RaceTrackController.Center = position;
            UpdateSubgroupPositions(forwardPosition);
            _forwardPosition = forwardPosition;
        }

        private void OnSubgroupsChanged(CandidateSubgroup[] subgroups)
        {
            for (var i = subgroups.Length; i < _subgroups.Count; i++)
            {
                var controller = _subgroups[i];
                controller.Subgroup = null;
                controller.HideImmediate();
            }

            for (var i = 0; i < subgroups.Length; i++)
            {
                if (i >= _subgroups.Count) _subgroups.Add(CreateSubgroup());
                var controller = _subgroups[i];
                var subgroup = subgroups[i];
                controller.Subgroup = subgroup;
                if (!controller.IsVisible) controller.Show();
            }

            UpdateSubgroupPositions(_forwardPosition);
        }

        private void OnLayoutChanged(CandidateGroupLayout layout)
        {
            if (layout == null) return;
            var rows = layout.Rows;
            var layoutConfiguration = Group.LayoutConfiguration;
            foreach (var g in _subgroups.Where(g => g.Subgroup != null))
            {
                var groupRows = rows.Where(r => r.Subgroup == g.Subgroup).ToList();
                var rowCount = groupRows.Count;
                var show = rowCount > 0;

                var subgroupPosition = groupRows.Select(r => r.RelativeForwardPosition).DefaultIfEmpty(0).Max();
                var spacing = layoutConfiguration.GroupSpacing;
                g.RelativeForwardPosition = subgroupPosition + spacing / 2;
                g.Length = rowCount * layoutConfiguration.SlotSize;

                if (show && !g.IsVisible) g.Show();
                else if (!show && g.IsVisible) g.HideImmediate();
            }

            UpdateSubgroupPositions(_forwardPosition);
        }

        private void UpdateSubgroupPositions(float forwardPosition)
        {
            foreach (var g in _subgroups.Where(g => g.Subgroup != null && g.IsVisible))
            {
                g.ForwardPosition = forwardPosition + g.RelativeForwardPosition;
            }
        }

        public RaceCandidateController[] GetBestCandidates(int count) => GetBestCandidates(Group.Groups, count);

        public RaceCandidateController[] GetBestCandidates(CandidateSubgroup[] groups, int count)
        {
            var bestGroup = groups.FirstOrDefault();
            var bestCandidates = (bestGroup != null && bestGroup.Candidates.Length >= count
                    ? bestGroup.Candidates.Where(c => c.IsAlive).Take(count)
                    : Group.Candidates.Any(c => c.AgreementScore > 0)
                        ? Group.Candidates.Where(c => c.IsAlive)
                            .OrderByDescending(c => c.AgreementScore + c.match / 100)
                        : Group.Candidates.Where(c => c.IsAlive).Take(Mathf.Min(Group.Candidates.Length, count))
                )
                .ToList();
            return RaceController.CandidateControllers
                .Where(c => bestCandidates.Any(bc => bc == c.Candidate))
                .ToArray();
        }

        public StrollState Stroll()
        {
            var result = new StrollState(this);
            StateMachine.State = result;
            return result;
        }

        public ReactToAnswerState ReactToAnswer(Question[] questions,
            QuestionAnswer answer)
        {
            var question = questions.Last();
            var generator = PickCourseGenerator(question, answer);
            //Debug.Log("Course: " + generator.name);
            if (generator == null) return null;

            var result = new ReactToAnswerState(this, generator, questions, answer);
            StateMachine.State = result;
            return result;
        }

        private RaceObstacleCourseGeneratorAsset PickCourseGenerator(Question question, QuestionAnswer answer)
        {
            var questionAxis = question.axis;
#if UNITY_EDITOR
            var keyboard = Keyboard.current;
            var hotkeyedCourseGenerators = RaceController.ObstacleCourseGenerators.ToDictionary(
                    g => g,
                    g => keyboard.FindKeyOnCurrentKeyboardLayout(g.Hotkey.ToString())
                )
                .Where(e => e.Value != null && e.Value.isPressed)
                .Select(e => e.Key)
                .ToList();
            if (hotkeyedCourseGenerators.Count > 0)
            {
                return hotkeyedCourseGenerators[Random.Range(0, hotkeyedCourseGenerators.Count)];
            }
#endif

            var availableCourseGenerators = RaceController.ObstacleCourseGenerators
                .Where(g => questionAxis.Any(qa =>
                        qa.axis == (int) g.AxisAssociation.axis &&
                        ((qa.value == 1) == (answer.answer >= 50)) == g.AxisAssociation.aligned
                    )
                )
                .ToList();

            if (availableCourseGenerators.Count == 0)
            {
                availableCourseGenerators.AddRange(RaceController.ObstacleCourseGenerators.Where(g =>
                    g.AxisAssociation.axis == SmartSpiderAxisAssociation.Axis.Any));
            }

            // Debug.Log("Answer: " + answer.answer + "\n" +
            //           "Axis: \n" + string.Join("\n", question.axis.Select(a => "- " + a.axis + ": " + a.value)));
            // Debug.Log("Available Courses: " + string.Join(", ", availableCourseGenerators.Select(c => c.name)));

            if (availableCourseGenerators.Count == 0)
            {
                Debug.LogError("No obstacle course generators available.");
                return null;
            }

            return availableCourseGenerators[Random.Range(0, availableCourseGenerators.Count)];
        }

        public void ShowSubgroups() => ShowSubgroups(true);
        public void HideSubgroups() => ShowSubgroups(false);

        public void ShowSubgroups(bool show)
        {
            foreach (var s in _subgroups.Where(s => s.IsVisible != show)) s.Show(show);
        }

        private CandidateSubgroupController CreateSubgroup()
        {
            var parent = RaceController.RaceTrackController.CandidatesAnchor;
            var instanceObject = Instantiate(_subgroupPrefab.gameObject, parent, false);
            var instance = instanceObject.GetComponent<CandidateSubgroupController>();
            instance.Initialize(this);
            return instance;
        }
    }
}