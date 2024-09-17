using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using System.IO;
using System;

public class SaveManager : Singleton<SaveManager>
{
    private string baseSavePath = $"{Application.dataPath}/Saves";
    public const string SAVE_FILE_EXTENSION = "json";

    public string BaseSavePath { get => baseSavePath; }

    void Start()
    {
        if (!Directory.Exists(BaseSavePath))
        {
            Directory.CreateDirectory(BaseSavePath);
        }
    }
    public void Save<T>(T obj, string fileName)
    {
        string filePath = $"{baseSavePath}/{fileName}.{SAVE_FILE_EXTENSION}";
        string jsonData = JsonConvert.SerializeObject(obj);

        File.WriteAllText(filePath, jsonData);
    }

    public T Load<T>(string fileName, bool isFullPath = false)
    {
        string filePath;
        if (isFullPath)
        {
            filePath = fileName;
        }
        else
        {
            filePath = $"{baseSavePath}/{fileName}.{SAVE_FILE_EXTENSION}";

        }
        string data;

        if (File.Exists(filePath))
        {
            data = File.ReadAllText(filePath);
            Debug.Log(data);
        }
        else
        {
            Debug.Log("File not found");
            return default(T);
        }

        return JsonConvert.DeserializeObject<T>(data);
    }

    public string[] FindSaveFiles()
    {
        return FindFilesInPath(BaseSavePath, SAVE_FILE_EXTENSION);
    }

    public string[] FindFilesInPath(string folderPath, string fileType = "json")
    {
        // Verifica che la cartella esista
        if (Directory.Exists(folderPath))
        {
            // Recupera tutti i file nella cartella
            string[] files = Directory.GetFiles(folderPath, $"*.{SAVE_FILE_EXTENSION}");

            // Mostra i file recuperati nella Console di Unity
            foreach (string file in files)
            {
                Debug.Log("File trovato: " + file);
            }
            return files;
        }
        else
        {
            Debug.Log("La cartella non esiste: " + folderPath);
            return null;
        }
    }
}
