public static class SaveMaster
{
    public static void ResetSaveData()
    {
        FileManager.Instance.SaveData("Levels/Level0", "leveldata",
        new LevelInfo
        (
            "Tutorial",
            "Level0",
            snowflakeAmount: 1,
            collectedSnowflakes: 0,
            id: 0,
            completed: false
        ).Serialize());
        FileManager.Instance.SaveData("Levels/Level1", "leveldata",
        new LevelInfo
        (
            "Level 1",
            "Level1",
            snowflakeAmount: 1,
            collectedSnowflakes: 0,
            id: 1,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("Levels/Level2", "leveldata",
        new LevelInfo
        (
            "Level 2",
            "Level2",
            snowflakeAmount: 3,
            collectedSnowflakes: 0,
            id: 2,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("Levels/Level3", "leveldata",
        new LevelInfo
        (
            "Level 3", 
            "Level3",
            snowflakeAmount: 3,
            collectedSnowflakes: 0,
            id: 3,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("Levels/LevelDevDumi", "leveldata",
        new LevelInfo
        (
            "Dev [Dumi]",
            "LevelDevDumi",
            snowflakeAmount: 3,
            collectedSnowflakes: 0,
            id: 3,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("Levels/LevelDevKirserd", "leveldata",
        new LevelInfo
        (
            "Dev [Kirserd]",
            "LevelDevKirserd",
            snowflakeAmount: 3,
            collectedSnowflakes: 0,
            id: 3,
            completed: false
        ).Serialize());
    }
}