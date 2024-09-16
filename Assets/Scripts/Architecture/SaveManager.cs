using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using System.IO;

public class SaveManager : Singleton<SaveManager>
{
    public void Save<T>(T obj, string fileName)
    {
        // string filePath = Application.persistentDataPath + "/" + fileName + ".save";
        string filePath = Application.dataPath + "/" + fileName + ".save";
        string jsonData = JsonConvert.SerializeObject(obj);

        FileStream file;

        if (File.Exists(filePath)) file = File.OpenWrite(filePath);
        else file = File.Create(filePath);

        File.WriteAllText(filePath, jsonData);
        file.Close();
    }

    public T Load<T>(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName + ".save";
        string data;

        if (File.Exists(filePath))
        {
            data = File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("File not found");
            return default(T);
        }

        return JsonConvert.DeserializeObject<T>(data);
    }
}
