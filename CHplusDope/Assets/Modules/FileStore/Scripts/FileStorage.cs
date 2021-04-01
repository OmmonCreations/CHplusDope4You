using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FileStore
{
    public abstract class FileStorage
    {
        public abstract void Initialize();
        public abstract string GetAbsolutePath(string path);
    }
}