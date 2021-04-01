using UnityEngine;
using UnityEngine.UI;

namespace Essentials
{
    [RequireComponent(typeof(Canvas))]
    [ExecuteAlways]
    [AddComponentMenu("Layout/Device Aware Canvas Scaler", 102)]
    [DisallowMultipleComponent]
    public class DeviceAwareCanvasScaler : CanvasScaler
    {
        protected override void Awake()
        {
            if (Application.isPlaying)
            {
                var safeArea = Screen.safeArea.size;
                var xRatio = safeArea.x / Screen.width;
                var yRatio = safeArea.y / Screen.height;
                var referenceResolution = this.referenceResolution;

                this.referenceResolution = new Vector2(referenceResolution.x / xRatio, referenceResolution.y / yRatio);
            }

            base.Awake();
        }
    }
}