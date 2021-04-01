using Essentials;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProgressBars
{
    public class ProgressBar : MonoBehaviour, IProgressListener
    {
        [SerializeField] private Image _fill = null;
        [SerializeField] private TMP_Text _description = null;

        private float _fillAmount;
        
        private float FillAmount
        {
            get { return _fillAmount; }
            set
            {
                _fillAmount = value;
                _fill.fillAmount = value;
            }
        }
        
        public void OnProgress(float progress)
        {
            FillAmount = progress;
        }

        public void OnFinish()
        {
            FillAmount = 1;
        }

        public void SetLabel(string label)
        {
            _description.text = label;
        }
    }
}