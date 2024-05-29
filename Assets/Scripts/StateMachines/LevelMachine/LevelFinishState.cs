using Monoliths;
using Monoliths.Visualisators;
using System.Collections;
using UnityEngine;

public class LevelFinishState : LevelSubState
{
    public override void Enter()
    {
        base.Enter();

        MonolithMaster.Instance.StartCoroutine(FinishSequence());
    }

    private IEnumerator FinishSequence()
    {
        DataBridge.TryGetData<PopUpStackPacket>(PopUpVisualisator.POP_UP_STACK_DATA_ID).EncodedData.SetLock(true);

        var fade = Object.Instantiate (
            Resources.Load<GameObject>("Prefabs/Visualisators/Transitions/FadeIn"), 
            GameObject.FindGameObjectWithTag("GUI").transform.GetChild((int)RenderingLayer.LAYER3)
        );
        
        yield return new WaitForSeconds(0.8f);
        Object.Destroy(fade);

        var levelBuffer = DataBridge.TryGetData<LevelInfo>(LevelProgressObserver.LEVEL_INFO_BUFFER_DATA_ID);
        if (!levelBuffer.IsEmpty)
            FileManager.Instance.SaveData(
                $"Levels/{levelBuffer.EncodedData.AssetName}",
                "leveldata",
                levelBuffer.EncodedData.Serialize());

        GameStateMachine.Instance.Return();
    }
}
