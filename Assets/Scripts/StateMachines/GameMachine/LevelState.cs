using Monoliths;
using Monoliths.Player;
using Monoliths.Visualisators;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelState : GameState<LevelSubState>
{
    private GameObject _levelInstance;
    private GameObject _legendaGUI;
    private GameObject _tutorialsScreen;
    public void FinalizeLevel()
    {
        if (!SubStateMachine.CurrentIs<MidLevelState>())
            return;

        SubStateMachine.Next<LevelFinishState>();
    }
    public override void Enter()
    {
        AudioManager.Instance.Stop("Level0MT"); //Actually main menu music!

        base.Enter();

        (MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)] as PlayerMovement).ResetPosition();

        var selectedLevel = DataBridge.TryGetData<LevelInfo>(LevelSelectionState.SELECTED_LEVEL_DATA_ID);

        AudioManager.Instance.SetMusicGroup(selectedLevel.EncodedData.LevelName);
        AudioManager.Instance.PlayLevelMusic(selectedLevel.EncodedData.LevelName);
        AudioManager.Instance.PlayAmbient(selectedLevel.EncodedData.LevelName);

        if(selectedLevel.EncodedData.LevelID == 0)
            FinalizeInitialization();        
        else
        {
            _tutorialsScreen = Object.Instantiate(Resources.Load<GameObject>
                ("Prefabs/Tutorials/" + selectedLevel.EncodedData.LevelID),
                GameObject.FindGameObjectWithTag("GUI").transform.GetChild((int)RenderingLayer.LAYER3));

            Controls.Profile.Map.FirstContextualButton.started += OnTutorialFinished;
        }
    }

    private void OnTutorialFinished(InputAction.CallbackContext ctx)
    {
        Controls.Profile.Map.FirstContextualButton.started -= OnTutorialFinished;

        Object.Destroy(_tutorialsScreen);

        FinalizeInitialization();
    }

    private void FinalizeInitialization()
    {
        var selectedLevel = DataBridge.TryGetData<LevelInfo>(LevelSelectionState.SELECTED_LEVEL_DATA_ID);
        var prefab = selectedLevel.EncodedData.GetAsset();
        _levelInstance = Object.Instantiate(prefab);

        DataBridge.UpdateData(SnowflakeVisualisator.SNOWFLAKE_AMOUNT_DATA_ID, selectedLevel.EncodedData.SnowflakeAmount);

        DataBridge.UpdateData(PlayerMovement.SIMULATION_ENABLED_DATA_ID, true);
        SubStateMachine.NextNoExit<LevelStartState>();

        if (DataBridge.TryGetData<LevelInfo>(LevelSelectionState.SELECTED_LEVEL_DATA_ID).EncodedData.LevelID != 0)
            _legendaGUI = Object.Instantiate(Resources.Load<GameObject>("Prefabs/PopUps/Legenda"),
                   GameObject.FindGameObjectWithTag("GUI").transform.GetChild((int)RenderingLayer.LAYER2));
    }

    public override void Exit()
    {
        MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(false);
        MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(false);
        MonolithMaster.Instance.Monoliths[typeof(LevelProgressObserver)]?.SetActive(false);

        DataBridge.UpdateData(PlayerMovement.SIMULATION_ENABLED_DATA_ID, false);

        if (_legendaGUI != null)
            Object.Destroy(_legendaGUI);

        Object.Destroy(_levelInstance);

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.StopAmbient();
        AudioManager.Instance.SetDefaultMusicGroup();

        base.Exit();
    }
}
