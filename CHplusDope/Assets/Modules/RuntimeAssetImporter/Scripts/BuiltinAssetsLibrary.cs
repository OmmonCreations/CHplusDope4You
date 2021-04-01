using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RuntimeAssetImporter
{
    [CreateAssetMenu(fileName = "AssetLibrary", menuName = "RuntimeAssetImporter/Builtin Assets Library")]
    public class BuiltinAssetsLibrary : AssetsLibrary
    {
        [SerializeField] private Sprite[] _sprites = null;

        public override void LoadInto(AssetPack assetPack)
        {
            assetPack.PutAssets(_sprites
                .Select((sprite, index) => new SpriteAsset(index + 1, sprite))
                .ToArray()
            );
        }
    }
}