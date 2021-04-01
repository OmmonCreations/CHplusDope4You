using UnityEngine;

namespace RuntimeAssetImporter
{
    public abstract class AssetsLibrary : ScriptableObject
    {
        public abstract void LoadInto(AssetPack assetPack);
    }
}