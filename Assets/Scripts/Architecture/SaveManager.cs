using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using System.IO;
using System;

public class SaveManager : Singleton<SaveManager>
{
    public void Save<T>(T obj, string fileName)
    {
        // string filePath = Application.persistentDataPath + "/" + fileName + ".save";
        string filePath = Application.dataPath + "/" + fileName + ".json";
        string jsonData = JsonConvert.SerializeObject(obj);

        File.WriteAllText(filePath, jsonData);
    }

    public T Load<T>(string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName + ".json";
        string data;

        if (File.Exists(filePath))
        {
            data = File.ReadAllText(filePath);
            Debug.Log(data);
        }
        else
        {
            Debug.LogError("File not found");
            return default(T);
        }

        return JsonConvert.DeserializeObject<T>(data);
    }
}
