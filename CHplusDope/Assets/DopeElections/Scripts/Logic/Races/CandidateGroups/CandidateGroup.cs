using System.Collections.Generic;
using System.Linq;
using DopeElections.Races.GroupLayout;
using DopeElections.Races.RaceTracks;
using Essentials;

namespace DopeElections.Races
{
    /// <summary>
    /// Main coordinator object for crowd behaviour
    /// </summary>
    public class CandidateGroup
    {
        #region Delegates

        public delegate void PositionEvent(float position);

        public delegate void SubgroupsEvent(CandidateSubgroup[] groups);

        public delegate void LayoutEvent(CandidateGroupLayout layout);

        public event PositionEvent PositionChanged = delegate { };
        public event SubgroupsEvent SubgroupsChanged = delegate { };
        public event LayoutEvent LayoutChanged = delegate { };

        #endregion

        #region Static Fields

        private static readonly List<CandidateSlotRow> TempRows = new List<CandidateSlotRow>();

        #endregion

        #region Public Fields

        public RaceContext Context { get; }
        public RaceTrack RaceTrack { get; }
        public CandidateGroupComposition Composition { get; }
        public RaceCandidateConfiguration CandidateConfiguration { get; }
        public RaceCandidate[] Candidates { get; }

        /// <summary>
        /// The maximum score candidates can have without any answers from this marathon
        /// </summary>
        public int BaseMaxScore => Race.BaseMaxScore;

        /// <summary>
        /// The maximum scores can have at the current question in the marathon including the base max score
        /// </summary>
        public int MaxScore => Race.MaxScore;

        public CandidateGroupLayoutConfiguration LayoutConfiguration => Composition.LayoutConfiguration;

        public CandidateSubgroup[] Groups { get; private set; }

        #endregion

        #region Private Fields

        private float _position;
        private int _largestSubgroup;
        private CandidateGroupLayout _layout;

        #endregion

        #region Public Properties

        public IRace Race => Context.Race;
        public float Length => Layout != null ? Layout.Length : 0;

        public CandidateGroupLayout Layout
        {
            get => _layout;
            private set => ApplyLayout(value);
        }

        public float Position
        {
            get => _position;
            set => ApplyPosition(value);
        }

        #endregion

        public CandidateGroup(RaceContext context, RaceTrack raceTrack, CandidateGroupComposition composition,
            RaceCandidateConfiguration configuration)
        {
            Context = context;
            RaceTrack = raceTrack;
            Composition = composition;
            CandidateConfiguration = configuration;

            var candidates = context.Race.Candidates;
            Candidates = candidates;
            Groups = new[]
            {
                new CandidateSubgroup(null, candidates,
                    new CandidateSubgroup.GroupMeta(new MinMaxRange(0, 100), null, false))
            };
        }

        #region Data Modifiers

        private void ApplyPosition(float position)
        {
            /*
            var existingLayout = Layout;
            var layout = existingLayout != null && GroupLayoutSolver.TestLayout(this, position, existingLayout)
                ? existingLayout
                : GroupLayoutSolver.CalculateLayout(this, position, Groups);
            SetPositionWithLayout(position, layout);
            */
            SetPositionWithLayout(position, Layout);
        }

        private void ApplyLayout(CandidateGroupLayout layout)
        {
            _layout = layout;
            Position = Position;
            LayoutChanged(layout);
        }

        private void ApplyLayoutAt(float position)
        {
            var layout = Layout;
            if (layout == null) return;
            var anchors = layout.CalculateAnchors(position);
            var rows = layout.Rows;
            for (var y = 0; y < rows.Length; y++)
            {
                var row = rows[y];
                var candidates = row.Candidates;
                for (var x = 0; x < candidates.Length; x++)
                {
                    var candidate = candidates[x];
                    if (candidate == null) continue;
                    candidate.GroupAnchor = anchors[y][x];
                }
            }
        }

        #endregion

        #region Public Setters

        public void SetPositionWithLayout(float position, CandidateGroupLayout layout)
        {
            _position = position;
            var spacing = LayoutConfiguration.GroupSpacing;
            if (RaceTrack.MaxPosition < position + spacing) RaceTrack.MainGenerator.CreateParts(position + spacing);
            if (layout != Layout)
            {
                layout.ApplySlots();
                Layout = layout;
            }

            ApplyLayoutAt(position);
            PositionChanged(position);
        }

        public void SetGroups(CandidateSubgroup[] groups)
        {
            Groups = groups;
            _largestSubgroup = groups.Select(g => g.Candidates.Length).DefaultIfEmpty(0).Max();
            SubgroupsChanged(groups);
        }

        public void RecalculateGroups()
        {
            var context = Context;
            var candidates = Candidates;
            var composition = Composition;
            var relativeRaceIndex = Context.RelativeRaceIndex;
            var currentRaceProgress = Context.Race.Progress;
            var groups = GroupCompositionSolver.CalculateGroups(context, candidates, composition, relativeRaceIndex,
                currentRaceProgress);
            SetGroups(groups);
        }

        public void ResetState(float position)
        {
            var candidates = Candidates;
            var raceTrack = RaceTrack;
            var groups = Groups;

            var aliveCandidates = groups.SelectMany(g => g.Candidates).ToList();
            foreach (var candidate in Candidates)
            {
                candidate.IsAlive = aliveCandidates.Contains(candidate);
            }

            //Debug.Log(aliveCandidates.Count + " candidates are alive");

            raceTrack.MainGenerator.CreateParts(position);
            var raceTrackWidth = raceTrack.Width;
            var layout = GroupLayoutSolver.CalculateLayoutFixedWidth(this, raceTrackWidth, groups);
            SetPositionWithLayout(position, layout);

            foreach (var candidate in candidates)
            {
                candidate.ResetState();
            }
        }

        #endregion
    }
}