public struct LevelResult
{
    public string AssetPath;
    public byte StarAmount;

    public byte CollectedStars;
    public byte LevelID;
    public bool IsCompleted;

    public LevelResult(string assetPath, byte starAmount, byte collectedStars, byte id, bool completed)
    {
        AssetPath = assetPath;
        StarAmount = starAmount;
        
        CollectedStars = collectedStars;
        LevelID = id;
        IsCompleted = completed;
    }

    public LevelResult(LevelResult other)
    {
        AssetPath = other.AssetPath;
        StarAmount = other.StarAmount;

        CollectedStars= other.CollectedStars;
        LevelID= other.LevelID;
        IsCompleted= other.IsCompleted;
    }
}


