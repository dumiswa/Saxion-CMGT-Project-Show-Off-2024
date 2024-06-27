using Monoliths;
using Monoliths.Player;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuState : GameState
{
    private const float SECONDS_TILL_INACTIVE = 90f;
    private float _inactivityCounter = 0f;

    private GameObject _screen;
    private Coroutine _activityObserver;
    public override void Enter()
    {
        if (_activityObserver == null)
            _activityObserver = MonolithMaster.Instance.StartCoroutine(ActivityObserver());

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
    
    private IEnumerator ActivityObserver()
    {
        Controls.Profile.Map.FirstContextualButton.performed += ctx => _inactivityCounter = 0;
        Controls.Profile.Map.SecondContextualButton.performed += ctx => _inactivityCounter = 0;
        Controls.Profile.Map.RightDirectional.performed += ctx => _inactivityCounter = 0;
        Controls.Profile.Map.LeftDirectional.performed += ctx => _inactivityCounter = 0;
        Controls.Profile.Map.Menu.performed += ctx => _inactivityCounter = 0;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            _inactivityCounter += Time.deltaTime;

            if(_inactivityCounter >= SECONDS_TILL_INACTIVE)
            {
                _inactivityCounter = 0;
                if (GameStateMachine.Instance.CurrentIs<LevelState>())
                {
                    (GameStateMachine.Instance.Current as LevelState).SubStateMachine.Next<LevelFinishState>();
                    yield return new WaitForSeconds(0.9f);
                }
                SaveMaster.ResetSaveData();
                GameStateMachine.Instance.Next<MenuState>();
            }
        }
    }
}
