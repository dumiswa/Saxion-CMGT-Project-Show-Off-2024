using Monoliths;
using Monoliths.Visualisators;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroCutsceneState : GameState
{
    private Cutscene _cutscene;
    private GameObject _skip;
    public override void Enter()
    {
        var prefab = Resources.Load<Cutscene>("Prefabs/Cutscenes/Intro");
        _cutscene = Object.Instantiate(prefab, GameObject.FindGameObjectWithTag("GUI").transform.GetChild((int)RenderingLayer.LAYER2));
        _cutscene.OnCutsceneFinished += GameStateMachine.Instance.Next<LevelSelectionState>;
        _cutscene.OnCutsceneFinished += () => Controls.Profile.Map.FirstContextualButton.started -= SkipIntro;
        MonolithMaster.Instance.StartCoroutine(EnterSequence());
        base.Enter();
    }

    private IEnumerator EnterSequence()
    {
        _skip = _cutscene.transform.Find("Legenda").gameObject;
        _skip.SetActive(false);
        yield return new WaitForSeconds(2f);
        _skip.SetActive(true);
        Controls.Profile.Map.FirstContextualButton.started += SkipIntro;
    }

    private void SkipIntro(InputAction.CallbackContext ctx) 
    {
        _cutscene.OnCutsceneFinished?.Invoke();
    }

    public override void Exit()
    {
        Object.Destroy(_cutscene.gameObject);
        base.Enter();
    }
}
