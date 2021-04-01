using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using Essentials;
using Html;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Candidates
{
    public class CandidateProfileView : DopeElectionsView, IView
    {
        public override NamespacedKey Id => GenericViewId.CandidateProfile;

        [SerializeField] private LocalizedText _tabHeaderText = null;
        [SerializeField] private HtmlCanvas _htmlCanvas = null;
        [SerializeField] private ScrollRect _scrollRect = null;

        private bool _scrollDirty = true;

        private ViewsContainer Views { get; set; }
        private Candidate _candidate;

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        private void OnEnable()
        {
            if (_scrollDirty) ResetScrollPosition();
        }

        protected override void OnInitialize(ViewsContainer viewsContainer)
        {
            Views = viewsContainer;
            base.OnInitialize(viewsContainer);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _tabHeaderText.key = LKey.Views.CandidateProfile.Title;
            _scrollDirty = true;
            if (gameObject.activeInHierarchy) ResetScrollPosition();
        }

        public new void Open()
        {
            base.Open();
        }

        private void ApplyCandidate(Candidate candidate)
        {
            _candidate = candidate;

            _htmlCanvas.Html = GetProfileText(candidate);
        }

        private string GetProfileText(Candidate candidate)
        {
            var localization = Views.Localization;
            var parts = new List<KeyValuePair<LocalizationKey, string>>();
            if (!string.IsNullOrWhiteSpace(candidate.topics))
            {
                parts.Add(new KeyValuePair<LocalizationKey, string>(
                    LKey.Components.Candidate.Topics,
                    $"<p>{candidate.topics}</p>"
                ));
            }

            if (!string.IsNullOrWhiteSpace(candidate.slogan))
            {
                parts.Add(new KeyValuePair<LocalizationKey, string>(
                    LKey.Components.Candidate.Slogan,
                    $"<p>{candidate.slogan}</p>"
                ));
            }

            if (!string.IsNullOrWhiteSpace(candidate.education))
            {
                parts.Add(new KeyValuePair<LocalizationKey, string>(
                    LKey.Components.Candidate.Education,
                    $"<p>{candidate.education}</p>"
                ));
            }

            if (candidate.termsInOffice != null && candidate.termsInOffice.Length > 0)
            {
                parts.Add(new KeyValuePair<LocalizationKey, string>(
                    LKey.Components.Candidate.TermsInOffice,
                    "<p>" + string.Join("<br>", candidate.termsInOffice
                        .Select(t => t.ToFormattedString(localization))) + "</p>"
                ));
            }

            if (!string.IsNullOrWhiteSpace(candidate.vestedInterests))
            {
                parts.Add(new KeyValuePair<LocalizationKey, string>(
                    LKey.Components.Candidate.VestedInterests,
                    $"<p>{candidate.vestedInterests}</p>"
                ));
            }

            if (candidate.campaignBudget > 0 || !string.IsNullOrWhiteSpace(candidate.campaignBudgetComment))
            {
                var subparts = new List<string>();
                if (candidate.campaignBudget > 0) subparts.Add("<p><b>" + candidate.campaignBudget + ".-</b></p>");
                if (!string.IsNullOrWhiteSpace(candidate.campaignBudgetComment))
                {
                    subparts.Add("<p>" + candidate.campaignBudgetComment + "</p>");
                }

                parts.Add(new KeyValuePair<LocalizationKey, string>(
                    LKey.Components.Candidate.CampaignBudget,
                    string.Join("", subparts)
                ));
            }

            if (!string.IsNullOrWhiteSpace(candidate.hobbies))
            {
                parts.Add(new KeyValuePair<LocalizationKey, string>(
                    LKey.Components.Candidate.Hobbies,
                    $"<p>{candidate.hobbies}</p>"
                ));
            }

            if (parts.Count == 0)
            {
                parts.Add(new KeyValuePair<LocalizationKey, string>(
                    LKey.Components.Candidate.NoProfileLabel,
                    "<p>" + LocalizationUtility.ApplyReplacements(
                        localization.GetString(LKey.Components.Candidate.NoProfileDescription),
                        new Dictionary<string, string>
                        {
                            ["name"] = candidate.firstName
                        }) + "</p>"
                ));
            }

            return string.Join("", parts.Select(e => $"<h2>{localization.GetString(e.Key)}</h2>{e.Value}"));
        }

        private void ResetScrollPosition()
        {
            _scrollDirty = false;
            StartCoroutine(SetScrollPosition());
        }

        private IEnumerator SetScrollPosition()
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 1;
        }
    }
}