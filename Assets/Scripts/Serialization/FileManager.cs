using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Monoliths;
using static System.Collections.Generic.Dictionary<string, LevelInfo>;

public class FileManager : Monolith
{
    public static FileManager Instance { get; private set; }

    private string _saveDataPath;
    private Dictionary<string, LevelInfo> _loadedSaveData;

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

    public LevelInfo GetLevelInfo(string key) => _loadedSaveData[key];
    public ValueCollection GetLevelInfos() => _loadedSaveData.Values;

    /// <summary>
    /// Loads all files with the .savedata extension from the SaveData folder.
    /// </summary>
    /// <returns>A list of byte arrays representing the loaded data.</returns>
    private void LoadAllSaveData(ref Dictionary<string, LevelInfo> collection)
    {
        collection = new();
        string[] files = Directory.GetFiles(_saveDataPath, "*.savedata");

        foreach (string file in files)
        {
            try
            {
                byte[] data = File.ReadAllBytes(file);

                var levelName = file.Substring
                (
                    _saveDataPath.Length + 1, 
                    file.Length - ".savedata".Length - _saveDataPath.Length - 1
                );
                var level = new LevelInfo();
                level.Deserialize(data);

                collection.Add(levelName, level);
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
    public void SaveData(string fileName, byte[] data)
    {
        string filePath = Path.Combine(_saveDataPath, $"{fileName}.savedata");

        try
        {
            File.WriteAllBytes(filePath, data);
            Debug.Log($"File Manager | Data successfully saved to {filePath}");
        }
        catch (Exception e)
        {
            Debug.Log($"File Manager | Failed to save data to {filePath}: {e.Message}");
        }

        LoadAllSaveData(ref _loadedSaveData);
    }
}
