using Monoliths.Visualisators;
using UnityEngine;

public class EndingCutsceneState : GameState
{
    private Cutscene _cutscene;
    public override void Enter()
    {
        var prefab = Resources.Load<Cutscene>("Prefabs/Cutscenes/Ending");
        _cutscene = Object.Instantiate(prefab, GameObject.FindGameObjectWithTag("GUI").transform.GetChild((int)RenderingLayer.LAYER2));
        _cutscene.OnCutsceneFinished += GameStateMachine.Instance.Next<LevelSelectionState>;
        base.Enter();
    }

    public override void Exit()
    {
        Object.Destroy(_cutscene.gameObject);
        base.Enter();
    }
}
