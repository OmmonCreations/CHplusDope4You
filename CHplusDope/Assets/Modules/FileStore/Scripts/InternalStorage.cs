using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace FileStore
{
    public class InternalStorage : FileStorage
    {
        private string _defaultStreamingAssetsPath = null;
        private string StreamingAssetsPath => _defaultStreamingAssetsPath;

        public override void Initialize()
        {
#if UNITY_IOS
            _defaultStreamingAssetsPath = "file://" + Application.streamingAssetsPath;
#else
            _defaultStreamingAssetsPath = Application.streamingAssetsPath;
#endif
        }

        public void ReadAllText(string relativePath, Action<string> callback)
        {
            var absolutePath = GetAbsolutePath(relativePath);
            var request = UnityWebRequest.Get(absolutePath);
            var operation = request.SendWebRequest();
            operation.completed += result =>
            {
                var content = request.downloadHandler.text;
                callback(content);
            };
        }

        public override string GetAbsolutePath(string relativePath)
        {
            var result = !Path.IsPathRooted(relativePath)
                ? Path.Combine(StreamingAssetsPath, relativePath)
                : relativePath;
            return result.Replace("\\", "/");
        }
    }
}