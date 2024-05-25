using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Monoliths;
using static System.Collections.Generic.Dictionary<string, byte[]>;

public class FileManager : Monolith
{
    public static FileManager Instance { get; private set; }

    private string _saveDataPath;
    private Dictionary<string, byte[]> _loadedSaveData;

    public override void Defaults()
    {
        base.Defaults();
        _priority = 101;
    }

    public override bool Init()
    {
        _saveDataPath = Path.Combine(Application.dataPath, "SaveData");

        if (!Directory.Exists(_saveDataPath))
            Directory.CreateDirectory(_saveDataPath);

        LoadAllSaveData(ref _loadedSaveData);

        Instance = this;
        return base.Init();
    }

    public byte[] GetSaveData(string key) => _loadedSaveData[key];
    public ValueCollection GetAllSaveData() => _loadedSaveData.Values;
    public byte[][] GetAllSaveDataOfExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            Debug.LogWarning("File Manager | Provided extension is null or empty.");
            return Array.Empty<byte[]>();
        }
        List<byte[]> result = new();

        foreach (var datagramm in _loadedSaveData)
            if (datagramm.Key.Split('.')[^1] == extension)
                result.Add(datagramm.Value);

        return result.ToArray();
    }
    public (string key, byte[] bytes)[] GetAllSaveDataPairsOfExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            Debug.LogWarning("File Manager | Provided extension is null or empty.");
            return Array.Empty<(string, byte[])>();
        }
        List<(string key, byte[] bytes)> result = new();

        foreach (var datagramm in _loadedSaveData)
            if (datagramm.Key.Split('.')[^1] == extension)
                result.Add((datagramm.Key, datagramm.Value));

        return result.ToArray();
    }

    /// <summary>
    /// Loads all files with the .savedata extension from the SaveData folder.
    /// </summary>
    /// <returns>A list of byte arrays representing the loaded data.</returns>
    private void LoadAllSaveData(ref Dictionary<string, byte[]> collection)
    {
        collection = new();
        string[] files = Directory.GetFiles(_saveDataPath, "*.*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            try
            {
                byte[] data = File.ReadAllBytes(file);
                var key = file.Substring(_saveDataPath.Length + 1);

                collection.Add(key, data);
            }
            catch (Exception e)
            {
                Debug.Log($"File Manager | Failed to load file {file}: {e.Message}");
            }
        }
    }

    /// <summary>
    /// Saves a byte array to a file with the specified name in the SaveData folder.
    /// </summary>
    /// <param name="fileName">The name of the file (without extension).</param>
    /// <param name="data">The byte array to save.</param>
    public void SaveData(string fileName, string extension, byte[] data, bool needsReload = true)
    {
        string filePath = Path.Combine(_saveDataPath, $"{fileName}.{extension}");

        string[] folders = fileName.Split('/');
        for (int i = 0; i < folders.Length - 1; i++)
        {
            var sample = Path.Combine(_saveDataPath, folders[i]);
            if (!Directory.Exists(sample))
                Directory.CreateDirectory(sample);
        }

        try
        {
            File.WriteAllBytes(filePath, data);
            Debug.Log($"File Manager | Data successfully saved to {filePath}");
        }
        catch (Exception e)
        {
            Debug.Log($"File Manager | Failed to save data to {filePath}: {e.Message}");
        }

        if(needsReload)
            LoadAllSaveData(ref _loadedSaveData);
    }
}
