public static class SaveMaster
{
    public static void ResetSaveData()
    {
        FileManager.Instance.SaveData("Level0",
        new LevelInfo
        (
            "Level0",
            starAmount: 1,
            collectedStars: 0,
            id: 0,
            completed: false
        ).Serialize());
        FileManager.Instance.SaveData("Level1",
        new LevelInfo
        (
            "Level1",
            starAmount: 1,
            collectedStars: 0,
            id: 1,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("Level2",
        new LevelInfo
        (
            "Level2",
            starAmount: 3,
            collectedStars: 0,
            id: 2,
            completed: false
        ).Serialize());

        FileManager.Instance.SaveData("Level3",
        new LevelInfo
        (
            "Level3",
            starAmount: 3,
            collectedStars: 0,
            id: 3,
            completed: false
        ).Serialize());
    }
}