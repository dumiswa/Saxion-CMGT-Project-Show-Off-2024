using Monoliths;
using Monoliths.Player;
using UnityEngine;

public class LevelSelectionState : GameState
{
    public const string SELECTED_LEVEL_DATA_ID = "SelectedLevel";

    private LevelSelectionScreen _screen;
    public override void Enter()
    {
        if (GameStateMachine.Instance.Previous is not MenuState && 
            GameStateMachine.Instance.Previous is not IntroCutsceneState && 
            GameStateMachine.Instance.Previous is not EndingCutsceneState)
        {
            var selectedLevel = DataBridge.TryGetData<LevelInfo>(SELECTED_LEVEL_DATA_ID);
            if (!selectedLevel.IsEmpty)
            {
                if (selectedLevel.EncodedData.LevelID == 0)
                {
                    GameStateMachine.Instance.Next<IntroCutsceneState>();
                    return;
                }
                else if (selectedLevel.EncodedData.LevelID == 3)
                {
                    GameStateMachine.Instance.Next<EndingCutsceneState>();
                    return;
                }
            }
        }

        var screen = Resources.Load<LevelSelectionScreen>("Prefabs/Screens/LevelSelection");
        bool intro = false;
        LevelInfo introLevel = new();

        _screen = Object.Instantiate(screen, GameObject.FindGameObjectWithTag("GUI").transform.GetChild(0));
        foreach (var data in FileManager.Instance.GetAllSaveDataOfExtension("leveldata"))
        {
            var levelInfo = new LevelInfo();
            levelInfo.Deserialize(data);

            if(levelInfo.LevelID == 0 && !levelInfo.IsCompleted)
            {
                introLevel = levelInfo;
                intro = true;
                break;
            }

            _screen.AddLevelInfo(levelInfo);
        }

        MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(false);
        MonolithMaster.Instance.Monoliths[typeof(PlayerInteractor)]?.SetActive(false);
        MonolithMaster.Instance.Monoliths[typeof(LevelProgressObserver)]?.SetActive(false);

        DataBridge.UpdateData<byte>(SnowflakeVisualisator.SNOWFLAKE_AMOUNT_DATA_ID, 0);

        if (intro)
        {
            DataBridge.UpdateData(SELECTED_LEVEL_DATA_ID, introLevel);
            GameStateMachine.Instance.Next<LevelState>();
            return;
        }

        if (AudioManager.Instance != null && !AudioManager.Instance.IsPlaying("MainMenuMusic"))
            AudioManager.Instance.PlayMainMenuMusic();

        base.Enter();
    }

    public override void Exit()
    {
        if (_screen != null)
            Object.Destroy(_screen.gameObject);

        base.Exit();
    }
}
