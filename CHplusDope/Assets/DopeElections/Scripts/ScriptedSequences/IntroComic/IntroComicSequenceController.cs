using System.Linq;
using DopeElections.Localizations;
using DopeElections.ScriptedSequences.IntroComic.States;
using Localizator;
using UnityEngine;

namespace DopeElections.ScriptedSequences.IntroComic
{
    public class IntroComicSequenceController : CinematicController
    {
        internal delegate void Event();

        internal event Event FirstPartCompleted = delegate { };
        internal event Event SecondPartCompleted = delegate { };
        internal event Event ThirdPartCompleted = delegate { };
        internal event Event TransitionTo3dStarted = delegate { };

        [SerializeField] private Animator _firstPartAnimator = null;
        [SerializeField] private GameObject _firstPartObject = null;
        [SerializeField] private Animator _secondPartAnimator = null;
        [SerializeField] private GameObject _secondPartObject = null;
        [SerializeField] private Animator _thirdPartAnimator = null;
        [SerializeField] private GameObject _thirdPartObject = null;
        [SerializeField] private GameObject _backgroundObject = null;
        [SerializeField] private LocalizationScope _localizationScope = null;

        [SerializeField] private SpriteRenderer[] _fadeInRenderers = null;
        [SerializeField] private SpriteRenderer[] _fadeOutRenderers = null;

        [Header("Texts")] [SerializeField] private LocalizedText _dialogue01Text = null;
        [SerializeField] private LocalizedText _dialogue02Text = null;
        [SerializeField] private LocalizedText _dialogue03Text = null;
        [SerializeField] private LocalizedText _dialogue04Text = null;
        [SerializeField] private LocalizedText _dialogue05Text = null;
        [SerializeField] private LocalizedText _dialogue06Text = null;
        [SerializeField] private LocalizedText _dialogue07Text = null;
        [SerializeField] private LocalizedText _tapText = null;
        [SerializeField] private LocalizedText _dialogue08Text = null;
        [SerializeField] private LocalizedText _dialogue09Text = null;
        [SerializeField] private LocalizedText _votingRightText = null;
        [SerializeField] private LocalizedText _dialogue10Text = null;
        [SerializeField] private LocalizedText _dialogue11Text = null;
        [SerializeField] private LocalizedText _dialogue12Text = null;
        [SerializeField] private LocalizedText _dialogue13Text = null;
        [SerializeField] private LocalizedText _dialogue14Text = null;
        [SerializeField] private LocalizedText _dialogue15Text = null;
        [SerializeField] private LocalizedText _dialogue16Text = null;
        [SerializeField] private LocalizedText _dialogue17Text = null;
        [SerializeField] private LocalizedText _dialogue18Text = null;
        [SerializeField] private LocalizedText _dialogue19Text = null;
        [SerializeField] private LocalizedText _dialogue20Text = null;
        [SerializeField] private LocalizedText _dialogue21Text = null;

        internal GameObject Background => _backgroundObject;
        internal Animator FirstPartAnimator => _firstPartAnimator;
        internal GameObject FirstPartObject => _firstPartObject;
        internal Animator SecondPartAnimator => _secondPartAnimator;
        internal GameObject SecondPartObject => _secondPartObject;
        internal Animator ThirdPartAnimator => _thirdPartAnimator;
        internal GameObject ThirdPartObject => _thirdPartObject;

        internal SpriteRenderer[] FadeInRenderers => _fadeInRenderers;
        internal SpriteRenderer[] FadeOutRenderers => _fadeOutRenderers;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            FirstPartObject.SetActive(false);
            SecondPartObject.SetActive(false);
            ThirdPartObject.SetActive(false);
            gameObject.SetActive(false);

            _localizationScope.Localization = DopeElectionsApp.Instance.Localization;
            _dialogue01Text.key = LKey.ScriptedSequences.IntroComic.Dialogue01;
            _dialogue02Text.key = LKey.ScriptedSequences.IntroComic.Dialogue02;
            _dialogue03Text.key = LKey.ScriptedSequences.IntroComic.Dialogue03;
            _dialogue04Text.key = LKey.ScriptedSequences.IntroComic.Dialogue04;
            _dialogue05Text.key = LKey.ScriptedSequences.IntroComic.Dialogue05;
            _dialogue06Text.key = LKey.ScriptedSequences.IntroComic.Dialogue06;
            _dialogue07Text.key = LKey.ScriptedSequences.IntroComic.Dialogue07;
            _tapText.key = LKey.ScriptedSequences.IntroComic.Tap;
            _dialogue08Text.key = LKey.ScriptedSequences.IntroComic.Dialogue08;
            _dialogue09Text.key = LKey.ScriptedSequences.IntroComic.Dialogue09;
            _votingRightText.key = LKey.ScriptedSequences.IntroComic.VotingRight;
            _dialogue10Text.key = LKey.ScriptedSequences.IntroComic.Dialogue10;
            _dialogue11Text.key = LKey.ScriptedSequences.IntroComic.Dialogue11;
            _dialogue12Text.key = LKey.ScriptedSequences.IntroComic.Dialogue12;
            _dialogue13Text.key = LKey.ScriptedSequences.IntroComic.Dialogue13;
            _dialogue14Text.key = LKey.ScriptedSequences.IntroComic.Dialogue14;
            _dialogue15Text.key = LKey.ScriptedSequences.IntroComic.Dialogue15;
            _dialogue16Text.key = LKey.ScriptedSequences.IntroComic.Dialogue16;
            _dialogue17Text.key = LKey.ScriptedSequences.IntroComic.Dialogue17;
            _dialogue18Text.key = LKey.ScriptedSequences.IntroComic.Dialogue18;
            _dialogue19Text.key = LKey.ScriptedSequences.IntroComic.Dialogue19;
            _dialogue20Text.key = LKey.ScriptedSequences.IntroComic.Dialogue20;
            _dialogue21Text.key = LKey.ScriptedSequences.IntroComic.Dialogue21;
        }

        public void TriggerInteractablePartStarted()
        {
            Controls.Interactable = false;
            if(Controls.AnimationController.IsVisible) Controls.AnimationController.Hide();
        }

        public void TriggerFirstPartComplete()
        {
            FirstPartCompleted();
        }

        public void TriggerSecondPartComplete()
        {
            SecondPartCompleted();
        }

        public void TriggerThirdPartComplete()
        {
            ThirdPartCompleted();
        }

        public void TriggerTransitionTo3dStarted()
        {
            TransitionTo3dStarted();
        }

        protected override ScriptedSequenceState[] GetParts()
        {
            return new ScriptedSequenceState[]
            {
                new FirstPart(this),
                new SecondPart(this),
                new ThirdPart(this)
            };
        }
    }
}