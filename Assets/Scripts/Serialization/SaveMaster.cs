public static class SaveMaster
{
    public static void ResetSaveData()
    {
        FileManager.Instance.SaveData("Level0",
        new LevelInfo
        (
            "Tutorial",
            "Level0",
            starAmount: 1,
            collectedStars: 0,
            id: 0,
            completed: false
        ).Serialize());
        FileManager.Instance.SaveData("Level1",
        new LevelInfo
        (
            "Level 1",
            "Level1",
            starAmount: 1,
            collectedStars: 0,
            id: 1,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("Level2",
        new LevelInfo
        (
            "Level 2",
            "Level2",
            starAmount: 3,
            collectedStars: 0,
            id: 2,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("Level3",
        new LevelInfo
        (
            "Level 3", 
            "Level3",
            starAmount: 3,
            collectedStars: 0,
            id: 3,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("LevelDevDumi",
        new LevelInfo
        (
            "Dev [Dumi]",
            "LevelDevDumi",
            starAmount: 3,
            collectedStars: 0,
            id: 3,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("LevelDevKirserd",
        new LevelInfo
        (
            "Dev [Kirserd]",
            "LevelDevKirserd",
            starAmount: 3,
            collectedStars: 0,
            id: 3,
            completed: false
        ).Serialize());
    }
}