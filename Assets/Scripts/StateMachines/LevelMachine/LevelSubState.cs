using UnityEngine.InputSystem;

public abstract class LevelSubState : GameState 
{
    public static void QuickLevelFinish(InputAction.CallbackContext ctx)
    {
        if (DataBridge.TryGetData<LevelInfo>(LevelSelectionState.SELECTED_LEVEL_DATA_ID).EncodedData.LevelID != 0)
            (GameStateMachine.Instance.Current as LevelState).SubStateMachine.Next<LevelFinishState>();
    }
}
