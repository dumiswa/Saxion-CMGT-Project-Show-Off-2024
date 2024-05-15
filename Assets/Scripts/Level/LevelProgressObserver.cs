using Monoliths;
using System;
using UnityEngine;

public class LevelProgressObserver : Monolith
{
    public const string LEVEL_INFO_BUFFER_DATA_ID = "CurrentLevelInfoBuffer";

    private GameObject[] _stars;
    private GameObject[] _levelTargets;

    public override void Defaults()
    {
        base.Defaults();
        _priority = 101;                        // DUE TO TESTING SESSION, SHOULDNT BE 101, SET TO BE -1,
                                                // WHEN STARTING STATE WILL SWITCH TO MENU STATE
    }

    public override bool Init()
    {
        GameState.OnEnter += OnGameStateEnter;  // DUE TO TESTING SESSION, SHOULDN"T BE THERE,
                                                // REMOVE THE ROW WHEN STARTING STATE WILL SWITCH TO MENU STATE
        return base.Init();
    }
    private void Scan()
    {
        _stars = GameObject.FindGameObjectsWithTag("Star");
        _levelTargets = GameObject.FindGameObjectsWithTag("IceCrystal");

        foreach (var star in _stars)
        {
            (star.GetComponent<Collectable>() ??
             star.AddComponent<Collectable>()).OnCollison += HandleStarAcquired;
        }

        foreach (var target in _levelTargets)
        {
            (target.GetComponent<Collectable>() ??
             target.AddComponent<Collectable>()).OnCollison += HandleTargetAcquired;
        }

        InitializeLevelInfo();
    }

    private void OnEnable() => GameState.OnEnter += OnGameStateEnter; 
    private void OnDisable() => GameState.OnEnter -= OnGameStateEnter; 

    private void OnGameStateEnter(GameState current)
    {
        Debug.Log("havent");
        if (current is not LevelStartState startState)
            return;

        Scan();
    }

    private void HandleStarAcquired(GameObject star) => UpdateLevelInfo(addStars: 1);
    private void HandleTargetAcquired(GameObject levelTarget)
    {
        UpdateLevelInfo(levelAccomplished: true);
        FinalizeLevel();
    }

    private void InitializeLevelInfo() 
    {
        var selectedLevelInfo = DataBridge.TryGetData<LevelResult>(LevelSelectionState.SELECTED_LEVEL_DATA_ID);
        
        if (selectedLevelInfo.IsEmpty) 
        {
            _isActive = false;
            _status = "Couldn't Find Selected Level Data";
            return;
        }
        else
        {
            if (!_isActive)
                Init();
        }

        var levelBuffer = new LevelResult(selectedLevelInfo.EncodedData);
        DataBridge.UpdateData(LEVEL_INFO_BUFFER_DATA_ID, levelBuffer);
    }
    private void UpdateLevelInfo(byte addStars = 0, bool levelAccomplished = false)
    {
        var data = DataBridge.TryGetData<LevelResult>(LEVEL_INFO_BUFFER_DATA_ID);
        Debug.Log($"UPDATE TO [addstars: {addStars} ; levelAccomplished: {levelAccomplished}]");
        try
        {
            if (!data.IsEmpty)
            {
                if (!_isActive)
                    base.Init();

                LevelResult levelInfo = data.EncodedData;
                levelInfo.CollectedStars += addStars;

                if(levelAccomplished)
                    levelInfo.IsCompleted = true;

                DataBridge.UpdateData(LEVEL_INFO_BUFFER_DATA_ID, levelInfo);
            }
            else
            {
                _isActive = false;
                _status = $"Stored data was not found";
            }
        }
        catch (InvalidCastException)
        {
            if (_isActive)
            {
                _isActive = false;
                _status = $"Stored data was not of appropriate types";
            }
        }
    }

    private void FinalizeLevel()
    {
        if (!(GameStateMachine.Instance.Current is LevelState levelState))
            return;

        levelState.FinalizeLevel();
    }
}


