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

        yield return new WaitForSeconds(0.8f);

        MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(true);
        MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(true);
        MonolithMaster.Instance.Monoliths[typeof(LevelProgressObserver)]?.SetActive(true);

        Object.Destroy(fade);
        (GameStateMachine.Instance.Current as LevelState).SubStateMachine.Next<MidLevelState>();
    }
}
