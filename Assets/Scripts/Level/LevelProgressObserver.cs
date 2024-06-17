using Monoliths;
using System;
using UnityEngine;

public class LevelProgressObserver : Monolith
{
    public const string LEVEL_INFO_BUFFER_DATA_ID = "CurrentLevelInfoBuffer";
    public const string CURRENT_SNOWFLAKES_DATA_ID = "CurrentCollectedSnowflakes";
    public const string BOSS_LEVEL_FINISHED_ID = "BossLevelFinished";

    private GameObject[] _snowflakes;
    private GameObject[] _iceCrystals;

    public override void Defaults()
    {
        base.Defaults();
        _priority = -1;
    }
    public override bool Init()
    {
        return base.Init();
    }

    private void Scan()
    {
        _snowflakes = GameObject.FindGameObjectsWithTag("Snowflake");
        _iceCrystals = GameObject.FindGameObjectsWithTag("IceCrystal");

        foreach (var star in _snowflakes)
        {
            (star.GetComponent<Collectable>() ??
             star.AddComponent<Collectable>()).OnCollison += HandleSnowflakeAcquired;
        }

        foreach (var target in _iceCrystals)
        {
            (target.GetComponent<Collectable>() ??
             target.AddComponent<Collectable>()).OnCollison += HandleIceCrystalAcquired;
        }

        InitializeLevelInfo();
    }

    private void Update()
    {
        var bossData = DataBridge.TryGetData<bool>(BOSS_LEVEL_FINISHED_ID);
        if (bossData.EncodedData)
        {
            DataBridge.UpdateData(BOSS_LEVEL_FINISHED_ID, false);
            UpdateLevelInfo(levelAccomplished: true);
            FinalizeLevel();
        }
    }

    private void OnEnable() => GameState.OnEnter += OnGameStateEnter; 
    private void OnDisable() => GameState.OnEnter -= OnGameStateEnter; 

    private void OnGameStateEnter(GameState current)
    {
        if (current is not LevelStartState startState)
            return;

        Scan();
    }

    private void HandleSnowflakeAcquired(GameObject star) 
        => UpdateLevelInfo(addSnowflakes: 1);
    private void HandleIceCrystalAcquired(GameObject levelTarget)
    {
        UpdateLevelInfo(levelAccomplished: true);
        FinalizeLevel();
    }

    private void InitializeLevelInfo() 
    {
        var selectedLevelInfo = DataBridge.TryGetData<LevelInfo>(LevelSelectionState.SELECTED_LEVEL_DATA_ID);
        
        if (selectedLevelInfo.IsEmpty) 
        {
            IsActive = false;
            _status = "Couldn't Find Selected Level Data";
            return;
        }
        else
        {
            if (!IsActive)
                Init();
        }

        var levelBuffer = new LevelInfo(selectedLevelInfo.EncodedData);
        DataBridge.UpdateData(LEVEL_INFO_BUFFER_DATA_ID, levelBuffer);
        DataBridge.UpdateData(BOSS_LEVEL_FINISHED_ID, false);
    }
    private void UpdateLevelInfo(byte addSnowflakes = 0, bool levelAccomplished = false)
    {
        var data = DataBridge.TryGetData<LevelInfo>(LEVEL_INFO_BUFFER_DATA_ID);
        try
        {
            if (!data.IsEmpty)
            {
                if (!IsActive)
                    base.Init();

                LevelInfo levelInfo = data.EncodedData;
                levelInfo.CollectedSnowflakes += addSnowflakes;

                if(levelAccomplished)
                    levelInfo.IsCompleted = true;

                DataBridge.UpdateData(LEVEL_INFO_BUFFER_DATA_ID, levelInfo);
                DataBridge.UpdateData(CURRENT_SNOWFLAKES_DATA_ID, levelInfo.CollectedSnowflakes);
            }
            else
            {
                IsActive = false;
                _status = $"Stored data was not found";
            }
        }
        catch (InvalidCastException)
        {
            if (IsActive)
            {
                IsActive = false;
                _status = $"Stored data was not of appropriate types";
            }
        }
    }

    private void FinalizeLevel()
    {
        if (GameStateMachine.Instance.Current is not LevelState levelState)
            return;
        levelState.FinalizeLevel();
    }
}


