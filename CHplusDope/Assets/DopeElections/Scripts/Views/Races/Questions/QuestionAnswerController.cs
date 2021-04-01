using AnimatedObjects;
using DopeElections.Answer;
using Localizator;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DopeElections.Races
{
    public class QuestionAnswerController : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        [SerializeField] private LocalizedText _labelText = null;
        [SerializeField] private Image _normalFillImage = null;
        [SerializeField] private Image _checkedFillImage = null;
        [SerializeField] private Image _borderImage = null;
        [SerializeField] private ToggleablePanelController _panelController = null;

        private QuestionAnswer _answer;

        public Image NormalFillImage => _normalFillImage;
        public Image CheckedFillImage => _checkedFillImage;
        public Image BorderImage => _borderImage;

        public ToggleablePanelController PanelController => _panelController;

        public LocalizationKey Label
        {
            get => _labelText.key;
            set => _labelText.key = value;
        }

        public bool Selected
        {
            get => _checkedFillImage.gameObject.activeSelf;
            set => ApplySelected(value);
        }

        public QuestionAnswer Answer
        {
            get => _answer;
            set => ApplyAnswer(value);
        }

        public UnityEvent<bool> onSelected { get; } = new UnityEvent<bool>();

        private void Awake()
        {
            _button.onClick.AddListener(Select);
        }

        private void Select()
        {
            Selected = true;
        }

        private void ApplyAnswer(QuestionAnswer answer)
        {
            _answer = answer;
        }

        private void ApplySelected(bool selected)
        {
            SetSelectedWithoutNotify(selected);
            onSelected.Invoke(selected);
        }

        public void SetSelectedWithoutNotify(bool selected)
        {
            _checkedFillImage.gameObject.SetActive(selected);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}