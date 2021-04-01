using System;
using System.Collections.Generic;
using System.Linq;
using RuntimeAssetImporter.Extensions;
using UnityEngine;

namespace RuntimeAssetImporter
{
    public class AssetPack
    {
        private readonly List<GenericAssetCategory> _storedCategories = new List<GenericAssetCategory>();
        
        public T GetAsset<T>(int key, int fallbackKey = default) where T : IAsset
        {
            var category = GetCategory<T>();
            var result = category != default ? category.Assets.FirstOrDefault(a => a.Key == key) : default;
            return result != null || fallbackKey == default ? result : GetAsset<T>(fallbackKey);
        }

        public T GetAsset<T>(Func<T,bool> filter) where T : IAsset
        {
            var category = GetCategory<T>();
            return category.Assets.FirstOrDefault(filter);
        }

        public T[] GetAssets<T>() where T : IAsset
        {
            var category = GetCategory<T>();
            return category != null ? category.Assets : new T[0];
        }

        public T[] GetAssets<T>(Func<T,bool> filter) where T : IAsset
        {
            var category = GetCategory<T>();
            return category.Assets.Where(filter).ToArray();
        }

        public void PutAsset<T>(T asset) where T : IAsset
        {
            PutAssets(asset);
        }

        public void PutAssets<T>(params T[] assets) where T : IAsset
        {
            var category = GetCategory<T>(true);
            if (category == null) _storedCategories.Add(new AssetCategory<T>(assets));
            else category.Append(assets);
        }

        public void RemoveAsset<T>(T asset) where T : IAsset
        {
            var category = GetCategory<T>();
            if (category == null) return;
            category.Remove(asset);
        }

        private AssetCategory<T> GetCategory<T>(bool quiet = false) where T : IAsset
        {
            var category = _storedCategories.OfType<AssetCategory<T>>().FirstOrDefault();
            if (category == null && !quiet)
            {
                // Debug.LogWarning("[RuntimeAssetImporter] No assets of type " + typeof(T).Name + " found!");
            }

            return category;
        }

        public void Clear()
        {
            _storedCategories.Clear();
            Resources.UnloadUnusedAssets();
        }

        public void Remove()
        {
            Resources.UnloadUnusedAssets();
        }

        private abstract class GenericAssetCategory
        {
            public abstract GenericAssetCategory Clone();
            public abstract void Append(GenericAssetCategory category);
        }

        private class AssetCategory<T> : GenericAssetCategory where T : IAsset
        {
            public T[] Assets { get; private set; }

            public AssetCategory(T[] assets)
            {
                Assets = assets;
            }

            /// <summary>
            /// Appends elements in provided array, skipping those whose AbstractAsset.FullId is already present
            /// </summary>
            /// <param name="assets"></param>
            public void Append(IEnumerable<T> assets)
            {
                Assets = Assets.Concat(assets).DistinctBy(a => a.Key).ToArray();
            }

            public void Remove(T asset)
            {
                Assets = Assets.Where(a => !ReferenceEquals(a, asset)).ToArray();
            }

            public override void Append(GenericAssetCategory category)
            {
                Append(((AssetCategory<T>) category).Assets);
            }

            public override GenericAssetCategory Clone()
            {
                return new AssetCategory<T>(Assets);
            }
        }
    }
}