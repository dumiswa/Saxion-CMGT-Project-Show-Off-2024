using Monoliths;
using Monoliths.Player;
using Monoliths.Visualisators;
using System.Collections;
using UnityEngine;

public class LevelStartState : LevelSubState
{
    public override void Enter()
    {
        base.Enter();
        MonolithMaster.Instance.StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        var fade = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Visualisators/Transitions/FadeOut"),
                   GameObject.FindGameObjectWithTag("GUI").transform.GetChild((int)RenderingLayer.LAYER3));

        var cameraSequence = DataBridge.TryGetData<CameraSequence>(CameraActions.SEQUENCE_DATA_ID).EncodedData;

        cameraSequence.Add(new Vector3(0, 2, 0), 0.75f);
        cameraSequence.Add(new Vector3(0, 0, 0), 0.75f);

        cameraSequence.Add(new Vector2(45, -45), 0.75f);
        cameraSequence.Add(new Vector2(30, -45), 0.75f);

        cameraSequence.Play();

        yield return new WaitForSeconds(3.0f);

        MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(true);
        MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(true);
        MonolithMaster.Instance.Monoliths[typeof(LevelProgressObserver)]?.SetActive(true);

        Object.Destroy(fade);
        (GameStateMachine.Instance.Current as LevelState).SubStateMachine.Next<MidLevelState>();

        DataBridge.TryGetData<PopUpStackPacket>(PopUpVisualisator.POP_UP_STACK_DATA_ID).EncodedData.SetLock(false);
    }
}
