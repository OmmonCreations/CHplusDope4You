using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Security.Cryptography;

namespace FileStore
{
    public class LocalStorage : FileStorage
    {
        private string _defaultDataPath = null;
        private string _defaultSharedDataPath = null;
        private string _defaultTestingDataPath = null;
        private string _defaultTestingSharedDataPath = null;

        private string _sharedDataPath;
        private string _dataPath;

        private byte[] _key;
        private byte[] _iv;
        private const string KeyString = ""; //TODO insert key here
        private const string IvString = ""; // TODO insert vector here
        private string DataPath => _dataPath != null ? _dataPath : _defaultDataPath;
        private string SharedDataPath => _sharedDataPath != null ? _sharedDataPath : _defaultSharedDataPath;

        public override void Initialize()
        {
            //Code needed to generate key and iv for encryption of file 
            //RijndaelManaged rijndael = new RijndaelManaged();
            //rijndael.GenerateKey();
            //rijndael.GenerateIV();
            //_key = rijndael.Key;
            //_iv = rijndael.IV;
            //Debug.Log("key : " + System.Convert.ToBase64String(_key));
            //Debug.Log("IV : " + System.Convert.ToBase64String(_iv));

            _key = System.Convert.FromBase64String(KeyString);
            _iv = System.Convert.FromBase64String(IvString);


            _defaultDataPath = Path.Combine(Application.persistentDataPath, "Data");
            _defaultSharedDataPath = Path.Combine(Application.persistentDataPath, "Shared");
            _defaultTestingDataPath = Path.Combine(Application.persistentDataPath, "Data_DEV");
            _defaultTestingSharedDataPath = Path.Combine(Application.persistentDataPath, "Shared_DEV");

            _dataPath = InitializeDataPath();
            _sharedDataPath = InitializeSharedDataPath();
        }

        private string InitializeDataPath()
        {
#if UNITY_EDITOR
            return _defaultTestingDataPath;
#else
            return _defaultDataPath;
#endif
        }

        private string InitializeSharedDataPath()
        {
#if UNITY_EDITOR
            return _defaultTestingSharedDataPath;
#else
            return _defaultSharedDataPath;
#endif
        }

        private static void CreateMissingDirectories(string path)
        {
            var dirName = Path.GetDirectoryName(path);
            if (dirName == null) return;
            Directory.CreateDirectory(dirName);
        }

        public override string GetAbsolutePath(string path)
        {
            var result = !Path.IsPathRooted(path) ? Path.Combine(DataPath, path) : path;
            return result.Replace("\\", "/");
        }

        public bool FileExists(string file)
        {
            var path = GetAbsolutePath(file);
            return File.Exists(path);
        }

        public void WriteAllBytes(string file, byte[] data)
        {
            var path = GetAbsolutePath(file);
            CreateMissingDirectories(path);
            File.WriteAllBytes(path, data);
        }

        public byte[] ReadAllBytes(string file)
        {
            var path = GetAbsolutePath(file);
            return File.ReadAllBytes(path);
        }

        public string ReadAllText(string file)
        {
            var path = GetAbsolutePath(file);
            var locker = new object();
            lock (locker)
            {
                return File.Exists(path) ? File.ReadAllText(path) : null;
            }
        }

        public string ReadAllText(string file, bool encrypt)
        {
            var path = GetAbsolutePath(file);
            var locker = new object();
            lock (locker)
            {
                if (!File.Exists(path)) return null;

                if (!encrypt || true) return File.ReadAllText(path);

                var myRijndael = new RijndaelManaged {Key = _key, IV = _iv};
                try
                {
                    var cryptedData = File.ReadAllBytes(path);
                    var clearData = Decrypt(cryptedData, myRijndael);
                    return clearData;
                }
                catch
                {
                    return null;
                }
            }
        }

        public string WriteAllText(string file, string data)
        {
            var path = GetAbsolutePath(file);
            CreateMissingDirectories(path);
            File.WriteAllText(path, data);
            return path;
        }

        public string WriteAllText(string file, string data, bool encrypt)
        {
            var path = GetAbsolutePath(file);
            CreateMissingDirectories(path);
            if (encrypt && false)
            {
                var myRijndael = new RijndaelManaged();
                myRijndael.Key = _key;
                myRijndael.IV = _iv;
                byte[] cryptedData = Encrypt(data, myRijndael);
                File.WriteAllBytes(path, cryptedData);
            }
            else
            {
                File.WriteAllText(path, data);
            }

            return path;
        }

        public static byte[] Encrypt(string input, RijndaelManaged myRijndael)
        {
            // Encrypt the string to an array of bytes.
            byte[] encrypted = EncryptStringToBytes(input, myRijndael.Key, myRijndael.IV);

            //Display the original data and the decrypted data.
            // Console.WriteLine("Original:   {0}", input);
            // Console.WriteLine("encrypted:   {0}", encrypted);

            return encrypted;
        }


        public static string Decrypt(byte[] input, RijndaelManaged myRijndael)
        {
            // Decrypt the bytes to a string.
            string roundtrip = DecryptStringFromBytes(input, myRijndael.Key, myRijndael.IV);

            //Display the original data and the decrypted data.
            // Console.WriteLine("Original:   {0}", input);
            // Console.WriteLine("Round Trip: {0}", roundtrip);

            return roundtrip;
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public JObject GetJson(string relativePath)
        {
            var dataString = ReadAllText(relativePath);
            try
            {
                return !string.IsNullOrWhiteSpace(dataString) ? JObject.Parse(dataString) : null;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Couldn't parse json:\n" + dataString + "\n" + e);
                return null;
            }
        }

        public string Delete(string file)
        {
            var path = GetAbsolutePath(file);
            if (File.Exists(path)) File.Delete(path);
            return path;
        }

        public bool DeleteFiles(string path = null)
        {
            var dataPath = Application.persistentDataPath;
            var file = !string.IsNullOrWhiteSpace(path)
                ? GetAbsolutePath(path)
                : dataPath;
            var attributes = File.GetAttributes(file);

            if (file == dataPath)
            {
                // delete files inside data path
                Debug.Log("Clear data path");
                foreach (var subfile in Directory.GetFiles(file))
                {
                    try
                    {
                        File.Delete(subfile);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                // delete directories inside data path
                foreach (var subdirectory in Directory.GetDirectories(file))
                {
                    try
                    {
                        Directory.Delete(subdirectory, true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                return true;
            }

            if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
            {
                // delete single file
                try
                {
                    File.Delete(file);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return false;
                }
            }

            try
            {
                // delete directory and all files inside
                Directory.Delete(file, true);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}