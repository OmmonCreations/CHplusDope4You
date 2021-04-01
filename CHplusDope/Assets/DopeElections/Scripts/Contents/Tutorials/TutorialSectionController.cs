using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Tutorials
{
    public class TutorialSectionController : MonoBehaviour
    {
        [SerializeField] private LocalizedText _titleText = null;
        [SerializeField] private LocalizedText _descriptionText = null;
        [SerializeField] private Image _tutorialImage = null;

        public LocalizedText Title => _titleText;
        public LocalizedText Text => _descriptionText;
        public Image Image => _tutorialImage;
    }
}