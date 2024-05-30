using Monoliths;
using Monoliths.Player;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionState : GameState
{
    public const string SELECTED_LEVEL_DATA_ID = "SelectedLevel";

    private List<LevelDisplay> _levels = new();

    private Transform _screen;
    public override void Enter()
    {
        var screen = Resources.Load<GameObject>("Prefabs/Screens/LevelSelection");
        var prefab = Resources.Load<GameObject>("Prefabs/Visualisators/LevelDisplay");

        _screen = Object.Instantiate(screen, GameObject.FindGameObjectWithTag("GUI").transform.GetChild(0)).transform;
        var content = _screen.GetChild(1).GetChild(0).GetChild(0);
        foreach (var data in FileManager.Instance.GetAllSaveDataOfExtension("leveldata"))
        {
            var levelInfo = new LevelInfo();
            levelInfo.Deserialize(data);

            var levelDisplay = Object.Instantiate(prefab,content).GetComponent<LevelDisplay>();

            levelDisplay.Index = _levels.Count;
            levelDisplay.LevelInfo = levelInfo;
            levelDisplay.Display();

            levelDisplay.OnLevelSelected += OnLevelSelected;

            _levels.Add(levelDisplay);
        }

        MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(false);
        MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(false);
        MonolithMaster.Instance.Monoliths[typeof(LevelProgressObserver)]?.SetActive(false);

        base.Enter();
    }
    private void OnLevelSelected(int index)
    {
        if (_levels.Count <= index)
            return;

        DataBridge.UpdateData(SELECTED_LEVEL_DATA_ID, _levels[index].LevelInfo);
        GameStateMachine.Instance.Next<LevelState>();
    }

    public override void Exit()
    {
        ClearLevels();
        Object.Destroy(_screen.gameObject);

        base.Exit();
    }

    private void ClearLevels()
    {
        foreach (var level in _levels)
            level.Clear();

        _levels.Clear();
    }
}
