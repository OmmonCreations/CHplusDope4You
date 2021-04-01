using UnityEngine;

namespace Spinners
{
    public abstract class Spinner : MonoBehaviour
    {
        [SerializeField] private float _interval = 1f;
        
        private float _t;
        
        protected void Update()
        {
            _t += Time.deltaTime / _interval;
            if (_t >= 1) _t -= 1;
            UpdateSpinner(_t);
        }

        protected abstract void UpdateSpinner(float t);
    }
}
