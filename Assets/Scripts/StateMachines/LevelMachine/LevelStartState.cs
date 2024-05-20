using Monoliths;
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

        yield return new WaitForSeconds(1.2f);

        Object.Destroy(fade);
        (GameStateMachine.Instance.Current as LevelState).SubStateMachine.Next<MidLevelState>();
    }
}
