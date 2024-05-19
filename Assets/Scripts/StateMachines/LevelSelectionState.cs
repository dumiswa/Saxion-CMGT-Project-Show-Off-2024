using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionState : GameState
{
    public const string SELECTED_LEVEL_DATA_ID = "SelectedLevel";

    private List<LevelDisplay> _levels = new();

    public override void Enter()
    {
        foreach (var levelInfo in FileManager.Instance.GetLevelInfos())
        {
            var levelDisplay = new GameObject("LevelDisplay").AddComponent<LevelDisplay>();
            levelDisplay.Index = _levels.Count;
            levelDisplay.LevelInfo = levelInfo;
            levelDisplay.OnLevelSelected += OnLevelSelected;
            _levels.Add(levelDisplay);
        }

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
