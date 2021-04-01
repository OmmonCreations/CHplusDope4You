using System;
using System.Collections;
using UnityEngine;

namespace Essentials
{
    public class SmartScreenCapture : MonoBehaviour
    {
        public Texture2D ScreenshotTexture { get; private set; }
        
        public bool Grayscale { get; set; }
        private Action<Texture2D> Callback { get; set; }
        
        private void Awake()
        {
            ScreenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        }

        private void Start()
        {
            StartCoroutine(UpdateScreenshotTexture(Grayscale));
        }

        public IEnumerator UpdateScreenshotTexture(bool grayscale)
        {
            yield return new WaitForEndOfFrame();
            RenderTexture transformedRenderTexture = null;
            RenderTexture renderTexture = RenderTexture.GetTemporary(
                Screen.width,
                Screen.height,
                24,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.Default,
                1);
            try
            {
                ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);
                transformedRenderTexture = RenderTexture.GetTemporary(
                    ScreenshotTexture.width,
                    ScreenshotTexture.height,
                    24,
                    RenderTextureFormat.ARGB32,
                    RenderTextureReadWrite.Default,
                    1);
                Graphics.Blit(
                    renderTexture,
                    transformedRenderTexture,
                    new Vector2(1.0f, -1.0f),
                    new Vector2(0.0f, 1.0f));
                RenderTexture.active = transformedRenderTexture;
                ScreenshotTexture.ReadPixels(
                    new Rect(0, 0, ScreenshotTexture.width, ScreenshotTexture.height),
                    0, 0);
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e);
                yield break;
            }
            finally
            {
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(renderTexture);
                if (transformedRenderTexture != null)
                {
                    RenderTexture.ReleaseTemporary(transformedRenderTexture);
                }
            }

            ScreenshotTexture.Apply();
            Callback(ScreenshotTexture);
            Remove();
        }

        private void Remove()
        {
            Destroy(gameObject);
        }

        public static SmartScreenCapture CaptureScreenshotAsTexture(Action<Texture2D> callback)
        {
            var instanceObject = new GameObject();
            var instance = instanceObject.AddComponent<SmartScreenCapture>();
            instance.Callback = callback;
            return instance;
        }
    }
}