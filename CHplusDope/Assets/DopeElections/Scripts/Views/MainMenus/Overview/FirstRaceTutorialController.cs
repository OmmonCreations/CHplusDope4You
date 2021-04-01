using System.Collections.Generic;
using AnimatedObjects;
using DopeElections.Localizations;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.MainMenus
{
    public class FirstRaceTutorialController : MonoBehaviour
    {
        [SerializeField] private LocalizedText _firstLineText = null;
        [SerializeField] private LocalizedText _secondLineText = null;
        [SerializeField] private LocalizedText _thirdLineText = null;
        [SerializeField] private ToggleableObjectController _animationController = null;
        [SerializeField] private Graphic[] _dimmableGraphics = null;
        [SerializeField] private Color _dimColor = Color.black;
        [SerializeField] private float _dimAmount = 0.5f;

        private Dictionary<Graphic, Color> _colorsBeforeDimmimng = new Dictionary<Graphic, Color>();

        public void Initialize()
        {
            _firstLineText.key = LKey.Views.Overview.FirstRaceTutorial.FirstLine;
            _secondLineText.key = LKey.Views.Overview.FirstRaceTutorial.SecondLine;
            _thirdLineText.key = LKey.Views.Overview.FirstRaceTutorial.ThirdLine;
            _animationController.HideImmediate();
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _animationController.Show(0.5f);
            foreach (var g in _dimmableGraphics)
            {
                if (_colorsBeforeDimmimng.TryGetValue(g, out var colorBeforeDimming))
                {
                    g.color = colorBeforeDimming;
                }
                else
                {
                    _colorsBeforeDimmimng[g] = g.color;
                }

                g.color = Color.Lerp(g.color, _dimColor, _dimAmount);
            }
        }

        public void Hide()
        {
            foreach (var g in _dimmableGraphics)
            {
                if (!_colorsBeforeDimmimng.TryGetValue(g, out var color)) continue;
                g.color = color;
            }
            gameObject.SetActive(false);
            _animationController.HideImmediate();
        }
    }
}