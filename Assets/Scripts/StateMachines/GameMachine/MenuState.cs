using UnityEngine;
using UnityEngine.InputSystem;

public class MenuState : GameState
{
    private GameObject _screen;

    public override void Enter()
    {
        Controls.Profile.Map.FirstContextualButton.performed += Next;
        Controls.Profile.Map.SecondContextualButton.performed += Next;

        _screen = Object.Instantiate(
            Resources.Load<GameObject>("Prefabs/Screens/Menu"), 
            GameObject.FindGameObjectWithTag("GUI").transform.GetChild(0));
        
        base.Enter();
    }

    public void Next(InputAction.CallbackContext ctx) 
        => GameStateMachine.Instance.Next<LevelSelectionState>();


    public override void Exit()
    {
        Controls.Profile.Map.FirstContextualButton.performed -= Next;
        Controls.Profile.Map.SecondContextualButton.performed -= Next;

        Object.Destroy(_screen);
        base.Exit();
    }
}
