﻿using Monoliths;
using Monoliths.Player;
using Monoliths.Visualisators;
using System.Collections;
using UnityEngine;

public class LevelFinishState : LevelSubState
{
    public override void Enter()
    {
        base.Enter();
        Controls.Profile.Map.Menu.started -= QuickLevelFinish;


        var player = GameObject.FindGameObjectWithTag("Player").transform;
        DataBridge.UpdateData(CameraActions.TARGET_DATA_ID, new CameraTarget(player, player, player));
        MonolithMaster.Instance.StartCoroutine(FinishSequence());
    }

    private IEnumerator FinishSequence()
    {
        yield return new WaitForEndOfFrame();
        var stack = DataBridge.TryGetData<PopUpStackPacket>(PopUpVisualisator.POP_UP_STACK_DATA_ID).EncodedData;
        stack.SetLock(true);
        stack.Clear();

        if(DataBridge.TryGetData<byte>(PlayerResources.CURRENT_LIVES_DATA_ID).EncodedData == 0) 
            yield return new WaitForSeconds(2f);

        var fade = Object.Instantiate (
            Resources.Load<GameObject>("Prefabs/Visualisators/Transitions/FadeIn"), 
            GameObject.FindGameObjectWithTag("GUI").transform.GetChild((int)RenderingLayer.LAYER3)
        );

        yield return new WaitForSeconds(0.8f);
        Object.Destroy(fade);

        var levelBuffer = DataBridge.TryGetData<LevelInfo>(LevelProgressObserver.LEVEL_INFO_BUFFER_DATA_ID);
        if (!levelBuffer.IsEmpty && levelBuffer.EncodedData.IsCompleted)
            FileManager.Instance.SaveData(
                $"Levels/{levelBuffer.EncodedData.AssetName}",
                "leveldata",
                levelBuffer.EncodedData.Serialize());

        GameStateMachine.Instance.Return();
    }
}
