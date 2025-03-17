using System;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Save
{
    public class SaveManager
    {
        const string extName = "cpp";
        private EncryptionAes encryption;

        public SaveManager()
        {
            encryption = new EncryptionAes();
        }

        public async UniTask Save<T>(T data) where T : SaveModel
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                string encryptedJson = encryption.EncryptString(json);
                string filePath = GetSaveFilePath(data.DataName + $".{extName}");

                await File.WriteAllTextAsync(filePath, encryptedJson); // ✅ Ghi file async

                Debug.Log("SAVE " + json);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async UniTask<T> Load<T>() where T : SaveModel, new()
        {
            try
            {
                T temp = new T();
                string filePath = GetSaveFilePath(temp.DataName + $".{extName}");
                if (File.Exists(filePath))
                {
                    string encryptedJson = await File.ReadAllTextAsync(filePath); // ✅ Đọc file async
                    string json = encryption.DecryptString(encryptedJson);
                    Debug.Log("LOAD " + json);

                    var deserializedData = (T)JsonConvert.DeserializeObject(json, typeof(T), new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });

                    return deserializedData;
                }

                temp.SetDefault();
                return temp;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error when loading save of {typeof(T)}: {e.Message}");
                T temp = new T();
                temp.SetDefault();
                return temp;
            }
        }
        public void Delete<T>() where T : SaveModel, new()
        {
            try
            {
                T temp = new T();
                string filePath = GetSaveFilePath(temp.DataName + $".{extName}");
                if (!File.Exists(filePath))
                    return;

                File.Delete(filePath);
                Debug.Log($"Deleted save file: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error when deleting save of {typeof(T)}: {e.Message}");
            }
        }

        private string GetSaveFilePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    }
}