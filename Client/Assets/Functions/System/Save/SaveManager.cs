using System;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class SaveManager
{
    const string extName = "cpp";
    private EncryptionAes encryption;

    public SaveManager()
    {
        encryption = new EncryptionAes();
    }

    public void Save<T>(T data) where T : SaveModel
    {
        try
        {
            // Serialize với TypeNameHandling để lưu metadata về subclass
            string json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            string encryptedJson = encryption.EncryptString(json);
            string filePath = GetSaveFilePath(data.DataName + $".{extName}");
            File.WriteAllText(filePath, encryptedJson);

            Debug.Log("SAVE " + json);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public T Load<T>() where T : SaveModel, new()
    {
        try
        {
            T temp = new T();
            string filePath = GetSaveFilePath(temp.DataName + $".{extName}");
            if (File.Exists(filePath))
            {
                string encryptedJson = File.ReadAllText(filePath);
                string json = encryption.DecryptString(encryptedJson);
                Debug.Log("LOAD " + json);

                // Deserialize với TypeNameHandling để parse subclass
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
            Debug.LogError($"Error when load save of {typeof(T)}: {e.Message}");
            T temp = new T();
            temp.SetDefault();
            return temp;
        }
    }

    private string GetSaveFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}