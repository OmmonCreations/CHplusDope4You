using System.Collections.Generic;
using System.Linq;
using AnimatedObjects;
using DopeElections.Answer;
using DopeElections.Localizations;
using Essentials;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Candidates
{
    public class ListPlacesController : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private LayoutElement _layoutElement = null;
        [SerializeField] private LocalizedText _mainText = null;
        [SerializeField] private LocalizedText _template = null;
        [SerializeField] private RectTransform _entriesArea = null;
        [SerializeField] private ToggleableObjectController _animationController = null;
        [SerializeField] private Color _interactableColor = Color.white;
        [SerializeField] private Color _nonInteractableColor = Color.white;
        [SerializeField] private Button _button = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private float _entryHeight = 63;
        [SerializeField] private float _baseHeight = 63;

        private readonly List<LocalizedText> _entries = new List<LocalizedText>();

        private Candidate _candidate;
        private bool _interactable;

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        public bool Interactable
        {
            get => _interactable;
            set => ApplyInteractable(value);
        }

        private bool IsExpanded => _animationController.IsVisible;

        private void Awake()
        {
            _button.onClick.AddListener(ToggleExpanded);
            _template.gameObject.SetActive(false);
        }

        private void ApplyCandidate(Candidate candidate)
        {
            _candidate = candidate;

            var listNumber = candidate.listNumber;
            var listPlaces = candidate.listPlaces != null
                ? candidate.listPlaces
                    .Where(p => p.position > 0)
                    .DistinctBy(p => p.number)
                    .ToList()
                : new List<ListPlace>();
            if (listNumber == 0 && listPlaces.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            if (!gameObject.activeSelf) gameObject.SetActive(true);

            var hasPlace = listPlaces.Count > 0;
            var hasListNumber = listNumber > 0;
            var hasMultiplePlaces = listPlaces.Count > 1;
            var interactable = hasMultiplePlaces;

            _mainText.key = GetMainTextKey(hasListNumber, hasPlace, hasMultiplePlaces);
            _mainText.SetVariable("list", listNumber.ToString());
            _mainText.SetVariable("number", hasPlace ? listPlaces[0].position.ToString() : "");
            _mainText.SetVariable("place", hasPlace
                ? listPlaces[0].number
                : "");
            _mainText.SetVariable("count", interactable ? (listPlaces.Count - 1).ToString() : "");

            _mainText.textComponent.color = interactable ? _interactableColor : _nonInteractableColor;
            _mainText.enabled = _mainText.key != default;

            UpdateEntries(listNumber, hasPlace ? listPlaces.Skip(1).ToList() : null);

            _animationController.HideImmediate();
            UpdateHeight();

            Interactable = interactable;
        }

        private LocalizationKey GetMainTextKey(bool hasListNumber, bool hasPlace, bool hasMultiplePlaces)
        {
            if (hasListNumber)
            {
                return hasPlace
                    ? hasMultiplePlaces
                        ? LKey.Components.Candidate.ListWithPlaces.Value
                        : LKey.Components.Candidate.ListWithPlace.Value
                    : LKey.Components.Candidate.List.Value;
            }

            return hasPlace
                ? hasMultiplePlaces
                    ? LKey.Components.Candidate.ListPlaces.Value
                    : LKey.Components.Candidate.ListPlace.Value
                : default;
        }

        private void ApplyInteractable(bool interactable)
        {
            _interactable = interactable;
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = interactable;
        }

        private void ToggleExpanded()
        {
            var expand = !_animationController.IsVisible;
            _animationController.Show(expand);
            _mainText.key = Interactable && !expand
                ? LKey.Components.Candidate.ListWithPlaces.Value
                : LKey.Components.Candidate.ListWithPlace.Value;
            UpdateHeight();
        }

        private void UpdateEntries(int listNumber, IReadOnlyCollection<ListPlace> listPlaces)
        {
            ClearEntries();
            if (listPlaces != null) _entries.AddRange(CreateEntries(listNumber, listPlaces));
        }

        private void ClearEntries()
        {
            foreach (var e in _entries) Destroy(e.gameObject);
            _entries.Clear();
        }

        private IEnumerable<LocalizedText> CreateEntries(int listNumber, IEnumerable<ListPlace> listPlaces)
        {
            return listPlaces.Select(p => CreateEntry(listNumber, p));
        }

        private LocalizedText CreateEntry(int listNumber, ListPlace listPlace)
        {
            var instanceObject = Instantiate(_template.gameObject, _entriesArea, false);
            var instance = instanceObject.GetComponent<LocalizedText>();
            instance.key = LKey.Components.Candidate.ListWithPlace.Value;
            instance.SetVariable("list", listNumber.ToString());
            instance.SetVariable("place", listPlace.number);
            instanceObject.SetActive(true);
            return instance;
        }

        private void UpdateHeight()
        {
            var height = _baseHeight + (IsExpanded ? _entries.Count * _entryHeight : 0);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _layoutElement.preferredHeight = height;
        }
    }
}