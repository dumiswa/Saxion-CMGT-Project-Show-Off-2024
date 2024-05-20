using Monoliths;
using Monoliths.Player;
using UnityEngine;

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

        var player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player is not null)
            player.position = Vector2.zero;

        var selectedLevel = DataBridge.TryGetData<LevelInfo>(LevelSelectionState.SELECTED_LEVEL_DATA_ID);

        if (selectedLevel.IsEmpty)
            return;

        var prefab = selectedLevel.EncodedData.GetAsset();
        _levelInstance = Object.Instantiate(prefab);

        MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(true);
        MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(true);
        MonolithMaster.Instance.Monoliths[typeof(LevelProgressObserver)]?.SetActive(true);

        DataBridge.UpdateData(PlayerMovement.SIMULATION_ENABLED_DATA_ID, true);
        SubStateMachine.NextNoExit<LevelStartState>();
    }

    public override void Exit()
    {
        MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(false);
        MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(false);
        MonolithMaster.Instance.Monoliths[typeof(LevelProgressObserver)]?.SetActive(false);

        DataBridge.UpdateData(PlayerMovement.SIMULATION_ENABLED_DATA_ID, false);
        Object.Destroy(_levelInstance);

        base.Exit();
    }
}
