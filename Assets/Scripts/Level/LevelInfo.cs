using System.IO;
using UnityEngine;

public struct LevelInfo : ISerializable
{
    public const string AssetPath = "Prefabs/Levels/";
    public string AssetName;

    public string LevelName;

    public byte SnowflakeAmount;
    public byte CollectedSnowflakes;
    public byte LevelID;
    public bool IsCompleted;

    public LevelInfo(string levelName, 
                     string assetPath,
                     byte snowflakeAmount, 
                     byte collectedSnowflakes,
                     byte id, 
                     bool completed)
    {
        LevelName = levelName;

        AssetName = assetPath;
        SnowflakeAmount = snowflakeAmount;
        
        CollectedSnowflakes = collectedSnowflakes;
        LevelID = id;
        IsCompleted = completed;
    }
    public LevelInfo(LevelInfo other)
    {
        LevelName = other.LevelName;

        AssetName = other.AssetName;
        SnowflakeAmount = other.SnowflakeAmount;

        CollectedSnowflakes= other.CollectedSnowflakes;
        LevelID= other.LevelID;
        IsCompleted= other.IsCompleted;
    }
    public LevelInfo(bool _ = false)
    {
        LevelName = "";

        AssetName = "";
        SnowflakeAmount = 0;

        CollectedSnowflakes = 0;
        LevelID = 0;
        IsCompleted = false;
    }

    public GameObject GetAsset() => Resources.Load<GameObject>(AssetPath + AssetName);

    public byte[] Serialize()
    {
        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);

        writer.Write(LevelName);
        writer.Write(AssetName);
        writer.Write(SnowflakeAmount);
        writer.Write(CollectedSnowflakes);
        writer.Write(LevelID);
        writer.Write(IsCompleted);

        return memoryStream.ToArray();
    }

    public void Deserialize(byte[] bytes)
    {
        using var memoryStream = new MemoryStream(bytes);
        using var reader = new BinaryReader(memoryStream);

        LevelName =           reader.ReadString();
        AssetName =           reader.ReadString();
        SnowflakeAmount =     reader.ReadByte();
        CollectedSnowflakes = reader.ReadByte();
        LevelID =             reader.ReadByte();
        IsCompleted =         reader.ReadBoolean();
    }
}

