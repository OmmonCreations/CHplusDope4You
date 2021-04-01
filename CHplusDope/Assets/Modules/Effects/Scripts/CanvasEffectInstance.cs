using UnityEngine;

namespace Effects
{
    public abstract class CanvasEffectInstance<T> : EffectInstance<T> where T : EffectData
    {
        [SerializeField] private RectTransform _rectTransform = null;

        public RectTransform RectTransform => _rectTransform;
    }
}