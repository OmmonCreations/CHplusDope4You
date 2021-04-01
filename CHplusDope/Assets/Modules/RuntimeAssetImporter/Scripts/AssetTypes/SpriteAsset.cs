using UnityEngine;

namespace RuntimeAssetImporter
{
    public class SpriteAsset : IAsset
    {
        public int Key { get; }
        public Sprite Sprite { get; }

        public SpriteAsset(int id, Sprite sprite)
        {
            Key = id;
            Sprite = sprite;
        }
    }
}