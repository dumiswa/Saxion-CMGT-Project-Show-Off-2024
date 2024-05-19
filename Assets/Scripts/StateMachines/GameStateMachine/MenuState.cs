using UnityEngine;

public class MenuState : GameState
{
    private Transform _gui;
    private GameObject _screen;

    public override void Enter()
    {
        Controls.Profile.Map.FirstContextualButton.performed += ctx => GameStateMachine.Instance.Next<LevelSelectionState>();
        Controls.Profile.Map.SecondContextualButton.performed += ctx => GameStateMachine.Instance.Next<LevelSelectionState>();

        _gui = GameObject.FindGameObjectWithTag("GUI").transform;
        _screen = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Screens/Menu"), _gui);
        
        base.Enter();
    }

    public override void Exit()
    {
        Controls.Profile.Map.FirstContextualButton.performed -= ctx => GameStateMachine.Instance.Next<LevelSelectionState>();
        Controls.Profile.Map.SecondContextualButton.performed -= ctx => GameStateMachine.Instance.Next<LevelSelectionState>();

        Object.Destroy(_screen);
        base.Exit();
    }
}
