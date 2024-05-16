using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    public delegate void OnEnterState(GameState state);
    public delegate void OnExitState(GameState state);

    public static OnEnterState OnEnter;
    public static OnExitState OnExit;

    public virtual void Enter() => OnEnter?.Invoke(this);
    public virtual void Exit() => OnExit?.Invoke(this);
}

public abstract class GameState<SubStateType> : GameState where SubStateType : GameState
{
    public AbstractStateMachine<SubStateType> SubStateMachine { get; protected set; } = new();
}

public class LevelSelectionState : GameState
{
    public const string SELECTED_LEVEL_DATA_ID = "SelectedLevel";

    private List<LevelDisplay> _levels = new();

    public override void Enter()
    {
        //LOAD LEVEL INFO
        //SHOW LEVELS

        //DEBUG
        _levels.Add(new GameObject("LevelDisplay").AddComponent<LevelDisplay>());
        _levels[0].LevelInfo = new LevelResult
        (
            "Prefabs/Levels/Level0",
            starAmount: 1, 
            collectedStars: 0,
            id: 0, 
            completed: false 
        );
        //DEBUG

        OnLevelSelected(0);
        base.Enter();
    }

    private void OnLevelSelected(int index)
    {
        DataBridge.UpdateData(SELECTED_LEVEL_DATA_ID, _levels[index].LevelInfo);
        GameStateMachine.Instance.Next<LevelState>();
    }

    public override void Exit()
    {
        ClearLevels();
        base.Exit();
    }

    private void ClearLevels()
    {
        foreach (var level in _levels)
            level.Clear();

        _levels.Clear();
    }
}

public class LevelState : GameState<LevelSubState>
{
    private GameObject _levelInstance;
    public void FinalizeLevel()
    {
        if (!SubStateMachine.CurrentIs<MidLevelState>())
            return;

        SubStateMachine.Next<LevelFinishState>();
    }
    public override void Enter()
    {
        base.Enter();

        var selectedLevel = DataBridge.TryGetData<LevelResult>(LevelSelectionState.SELECTED_LEVEL_DATA_ID);

        if (selectedLevel.IsEmpty)
            return;

        var prefab = Resources.Load<GameObject>(selectedLevel.EncodedData.AssetPath);
        _levelInstance = Object.Instantiate(prefab);

        SubStateMachine.NextNoExit<LevelStartState>();
    }

    public override void Exit()
    {
        base.Exit();
        Object.Destroy(_levelInstance);
    }
}
