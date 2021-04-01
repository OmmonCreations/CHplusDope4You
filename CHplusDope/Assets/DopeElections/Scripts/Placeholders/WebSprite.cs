using System;
using System.Collections.Generic;
using System.Linq;
using Essentials;
using UnityEngine;
using UnityEngine.Networking;

namespace DopeElections.Placeholders
{
    public class WebSprite
    {
        private const int MaxConcurrentDownloads = 3;
        private static int _currentDownloads = 0;
        private readonly static Dictionary<string, WebSprite> _cache = new Dictionary<string, WebSprite>();

        public delegate void LoadEvent();

        public event LoadEvent loaded = delegate { };

        public string path { get; }
        public Sprite sprite { get; private set; }
        public Sprite placeholder { get; }

        private Func<Vector2Int, Vector2Int> resampler;
        private Func<Vector2, Rect> viewport;
        private Func<Vector2, Vector2> pivot;

        private bool _loading = false;
        private bool _loaded = false;

        private WebSprite(string path, Func<Vector2Int, Vector2Int> resampler, Func<Vector2, Rect> viewport,
            Func<Vector2, Vector2> pivot,
            Sprite placeholderSprite = null)
        {
            this.path = path;
            this.resampler = resampler;
            this.viewport = viewport;
            this.pivot = pivot;
            placeholder = placeholderSprite;
        }

        private WebSprite(string path, Sprite sprite)
        {
            this.path = path;
            this.sprite = sprite;
            _loaded = true;
        }

        public void Assign(Action<Sprite> assignAction)
        {
            if (sprite)
            {
                assignAction(sprite);
                return;
            }

            assignAction(placeholder);
            loaded += () => assignAction(sprite);
        }

        private void Load()
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                // Debug.LogWarning("Cannot download sprite from path:\n" + path);
                _loaded = true;
                TryLoadNext();
                return;
            }

            if (_loading || _loaded) return;
            if (_currentDownloads >= MaxConcurrentDownloads) return;
            _currentDownloads++;
            _loading = true;

            // Debug.Log("Loading " + path);

            var request = UnityWebRequestTexture.GetTexture(path);
            var operation = request.SendWebRequest();
            operation.completed += o => ProcessRequestResult(request);
            ProcessRequestResult(request);
        }

        private void ProcessRequestResult(UnityWebRequest operation)
        {
            _loaded = true;
            _loading = false;

            _currentDownloads--;
            TryLoadNext();

            if (operation.result != UnityWebRequest.Result.Success)
            {
                // Debug.LogWarning("Couldn't download sprite from path:\n" + path);
                return;
            }

            var texture = DownloadHandlerTexture.GetContent(operation);
            if (resampler != null)
            {
                Resample(texture, resampler);
            }

            texture.name = path;

            var size = new Vector2(texture.width, texture.height);
            var viewport = this.viewport ?? GetOriginalViewport;
            var pivot = this.pivot ?? (s => new Vector2(0.5f, 0.5f));
            sprite = Sprite.Create(
                texture,
                viewport(size),
                pivot(size)
            );
            sprite.name = path;
            sprite.texture.wrapMode = TextureWrapMode.Clamp;
            loaded();
        }

        private void TryLoadNext()
        {
            var next = _cache.FirstOrDefault(e => !e.Value._loading && !e.Value._loaded).Value;
            if (next != null)
            {
                // Debug.Log("Try loading next sprite");
                next.Load();
            }
        }

        public static void Add(string url, Sprite sprite)
        {
            _cache[url] = new WebSprite(url, sprite);
        }

        public static WebSprite LoadSquare(string path, Sprite placeholder = null)
        {
            return Load(path, GetSquareViewport, placeholder);
        }

        public static WebSprite Load(string path, Sprite placeholderSprite = null)
        {
            var viewport = new Func<Vector2, Rect>(size => new Rect(Vector2.zero, size));
            return Load(path, viewport, placeholderSprite);
        }

        public static WebSprite Load(string path, Func<Vector2, Rect> viewport, Sprite placeholderSprite = null)
        {
            return Load(path, viewport, null, placeholderSprite);
        }

        public static WebSprite Load(string path, Func<Vector2, Rect> viewport,
            Func<Vector2, Vector2> pivot, Sprite placeholderSprite = null)
        {
            return Load(path, null, viewport, pivot, placeholderSprite);
        }

        public static WebSprite Load(string path, Func<Vector2Int, Vector2Int> resampler,
            Sprite placeholderSprite = null)
        {
            return Load(path, resampler, null, null, placeholderSprite);
        }

        public static WebSprite Load(string path, Func<Vector2Int, Vector2Int> resampler, Func<Vector2, Rect> viewport,
            Sprite placeholderSprite = null)
        {
            return Load(path, resampler, viewport, null, placeholderSprite);
        }

        public static WebSprite Load(string path, Func<Vector2Int, Vector2Int> resampler, Func<Vector2, Rect> viewport,
            Func<Vector2, Vector2> pivot, Sprite placeholderSprite = null)
        {
            if (_cache.TryGetValue(path, out var existing)) return existing;
            var result = new WebSprite(path, resampler, viewport, pivot, placeholderSprite);
            result.Load();
            if (!string.IsNullOrWhiteSpace(path)) _cache[path] = result;
            return result;
        }

        public static Rect GetOriginalViewport(Vector2 imageSize)
        {
            return new Rect(Vector2.zero, imageSize);
        }

        public static Rect GetSquareViewport(Vector2 imageSize)
        {
            var center = imageSize / 2;
            var size = Mathf.Min(imageSize.x, imageSize.y);
            var rectSize = new Vector2(size, size);
            return new Rect(center - rectSize / 2, rectSize);
        }

        public static Func<Vector2Int, Vector2Int> GetTextureResampler(Vector2Int limits)
        {
            return size => Fit(size, limits);
        }

        private static Vector2Int Fit(Vector2Int size, Vector2Int limits)
        {
            var xRatio = limits.x / (float) size.x;
            var yRatio = limits.y / (float) size.y;
            var ratio = Mathf.Min(xRatio, yRatio);
            return ratio < 1
                ? new Vector2Int(Mathf.RoundToInt(size.x * ratio), Mathf.RoundToInt(size.y * ratio))
                : size;
        }

        private static void Resample(Texture2D texture, Func<Vector2Int, Vector2Int> resampler)
        {
            var oldSize = new Vector2Int(texture.width, texture.height);
            var newSize = resampler(oldSize);
            if (newSize == oldSize) return;
            TextureUtility.Bilinear(texture, newSize.x, newSize.y);
        }
    }
}