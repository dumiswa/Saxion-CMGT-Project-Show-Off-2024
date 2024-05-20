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

        (MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)] as PlayerMovement).ResetPosition();

        var selectedLevel = DataBridge.TryGetData<LevelInfo>(LevelSelectionState.SELECTED_LEVEL_DATA_ID);

        if (selectedLevel.IsEmpty)
            return;

        var prefab = selectedLevel.EncodedData.GetAsset();
        _levelInstance = Object.Instantiate(prefab);

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
