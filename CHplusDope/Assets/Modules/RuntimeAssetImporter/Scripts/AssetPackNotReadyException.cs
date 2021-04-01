using System;

namespace RuntimeAssetImporter
{
    public class AssetPackNotReadyException : Exception
    {
        public AssetPackNotReadyException(string message) : base(message)
        {
            
        }
    }
}