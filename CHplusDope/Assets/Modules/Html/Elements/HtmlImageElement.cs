using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Html
{
    public class HtmlImageElement : HtmlElement
    {
        [SerializeField] private Image _image = null;
        [SerializeField] private TMP_Text _altText = null;

        protected override void GenerateBody(string body)
        {
            var src = GetAttribute("src");
            var alt = GetAttribute("alt");
            if (alt != null) _altText.text = alt;
            _altText.gameObject.SetActive(alt != null);

            if (src == null)
            {
                _image.enabled = false;
                return;
            }

            if (src.StartsWith("http"))
            {
                // load image from web
                LoadFromUrl(src);
                return;
            }

            var dataRegex = new Regex("^data:image/(png|jpe?g);base64, (.*)$");
            var dataMatch = dataRegex.Match(src);
            if (!string.IsNullOrWhiteSpace(dataMatch.Value))
            {
                var format = dataMatch.Groups[1].Value;
                var byteString = dataMatch.Groups[2].Value;
                LoadByteString(format, byteString);
                return;
            }

            if (src.StartsWith("/")) src = src.Substring(1);
            
#if UNITY_IOS
            var localPath = "file://" + Path.Combine(Application.streamingAssetsPath, src);
#else
            var localPath = Path.Combine(Application.streamingAssetsPath, src);
#endif
            localPath = localPath.Replace("\\", "/");
            LoadFromUrl(localPath);
        }

        private void LoadFromUrl(string url)
        {
            var query = UnityWebRequestTexture.GetTexture(url);
            var request = query.SendWebRequest();
            request.completed += result =>
            {
                if (query.error != null) Debug.LogError(url + ":\n" + query.error);
                var texture = DownloadHandlerTexture.GetContent(query);
                if (texture == null) return;
                ApplyTexture(texture);
            };
        }

        private void LoadByteString(string format, string byteString)
        {
            var bytes = System.Convert.FromBase64String(byteString);

            var texture = new Texture2D(16, 16, format == "png" ? TextureFormat.RGBA32 : TextureFormat.RGB24,
                false);
            try
            {
                texture.LoadRawTextureData(bytes);
            }
            catch
            {
                return;
            }

            ApplyTexture(texture);
        }

        private void ApplyTexture(Texture2D texture)
        {
            var width = texture.width;
            var height = texture.height;
            var sprite = Sprite.Create(texture,
                new Rect(new Vector2(0, 0), new Vector2(width, height)), new Vector2(0.5f, 0.5f));
            _image.sprite = sprite;
            _image.enabled = sprite;

            Width = width;
            Height = height;
        }
    }
}