using System.IO;
using UnityEngine;

public struct LevelInfo : ISerializable
{
    public const string AssetPath = "Prefabs/Levels/";
    public string AssetName;
    public byte StarAmount;

    public byte CollectedStars;
    public byte LevelID;
    public bool IsCompleted;

    public LevelInfo(string assetPath, byte starAmount, byte collectedStars, byte id, bool completed)
    {
        AssetName = assetPath;
        StarAmount = starAmount;
        
        CollectedStars = collectedStars;
        LevelID = id;
        IsCompleted = completed;
    }
    public LevelInfo(LevelInfo other)
    {
        AssetName = other.AssetName;
        StarAmount = other.StarAmount;

        CollectedStars= other.CollectedStars;
        LevelID= other.LevelID;
        IsCompleted= other.IsCompleted;
    }
    public LevelInfo(bool _ = false)
    {
        AssetName = "";
        StarAmount = 0;

        CollectedStars = 0;
        LevelID = 0;
        IsCompleted = false;
    }

    public GameObject GetAsset() => Resources.Load<GameObject>(AssetPath + AssetName);

    public byte[] Serialize()
    {
        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);

        writer.Write(AssetName);
        writer.Write(StarAmount);
        writer.Write(CollectedStars);
        writer.Write(LevelID);
        writer.Write(IsCompleted);

        return memoryStream.ToArray();
    }

    public void Deserialize(byte[] bytes)
    {
        using var memoryStream = new MemoryStream(bytes);
        using var reader = new BinaryReader(memoryStream);

        AssetName =      reader.ReadString();
        StarAmount =     reader.ReadByte();
        CollectedStars = reader.ReadByte();
        LevelID =        reader.ReadByte();
        IsCompleted =    reader.ReadBoolean();
    }
}

