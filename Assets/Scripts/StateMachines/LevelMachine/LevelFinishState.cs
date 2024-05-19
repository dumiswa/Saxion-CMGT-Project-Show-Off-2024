using UnityEngine;

public class LevelFinishState : LevelSubState
{
    public override void Enter()
    {
        base.Enter();

        //PLAY ANIM
        Debug.Log("Level Finished!");

        var levelBuffer = DataBridge.TryGetData<LevelInfo>(LevelProgressObserver.LEVEL_INFO_BUFFER_DATA_ID);
        if(!levelBuffer.IsEmpty)
            FileManager.Instance.SaveData(
                levelBuffer.EncodedData.AssetName,
                levelBuffer.EncodedData.Serialize());

        GameStateMachine.Instance.Return();
    }
}
