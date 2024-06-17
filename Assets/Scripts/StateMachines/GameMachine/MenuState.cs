using Monoliths.Player;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuState : GameState
{
    private GameObject _screen;

    public override void Enter()
    {
        LoadData();

        Controls.Profile.Map.FirstContextualButton.performed += Next;
        Controls.Profile.Map.SecondContextualButton.performed += Next;

        _screen = Object.Instantiate(
            Resources.Load<GameObject>("Prefabs/Screens/Menu"), 
            GameObject.FindGameObjectWithTag("GUI").transform.GetChild(0)
        );

        AudioManager.Instance.PlayMainMenuMusic();

        base.Enter();
    }

    private void LoadData()
    {
        var (key, bytes) = FileManager.Instance.GetAllSaveDataPairsOfExtension("resource")
                            .Where(pair => pair.key == PlayerResources.SAVED_LIVES_DATA_ID)
                            .FirstOrDefault();

        if (bytes is not null)
            DataBridge.UpdateData(PlayerResources.SAVED_LIVES_DATA_ID, bytes[0]);
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
