using Monoliths.Visualisators;
using UnityEngine;

namespace Monoliths.Player
{
    public class PlayerResources : Monolith
    {
        public const string SAVED_LIVES_DATA_ID = "svdplrlives";
        public const string CURRENT_LIVES_DATA_ID = "CurrentPlayerLives";

        private HPContainer _managed;
        private HPContainer _prefab;
        private byte _currentLives
        {
            get => _currentLivesBuffer;
            set
            {
                DataBridge.UpdateData(CURRENT_LIVES_DATA_ID, value);

                if(_managed)
                    _managed.SetCurrent(value);

                _currentLivesBuffer = value;

                if(value == 0)
                {
                    if (!(GameStateMachine.Instance.Current is LevelState levelState))
                        return;

                    DataBridge.UpdateData
                    (
                        LevelProgressObserver.LEVEL_INFO_BUFFER_DATA_ID,
                        new LevelInfo() { IsCompleted = false }
                    ); 

                    levelState.FinalizeLevel();
                }
            }
        }
        private byte _currentLivesBuffer;

        public override void Defaults()
        {
            _currentLives = 3;

            base.Defaults();
        }
        public override bool Init()
        {
            _prefab = Resources.Load<HPContainer>("Prefabs/Visualisators/InGame GUI/HPContainer");
            return base.Init();
        }

        protected virtual void Update()
        {
            var data = DataBridge.TryGetData<byte>(CURRENT_LIVES_DATA_ID);
            if (data.WasUpdated && data.EncodedData >= 0)
            {
                _currentLives = data.EncodedData;
                data.EncodedData = 1;
            }
        }

        private void OnLevelStart()
        {
            var data = DataBridge.TryGetData<byte>(SAVED_LIVES_DATA_ID);
            if (data.IsEmpty || data.EncodedData < 3)
            {
                DataBridge.UpdateData<byte>(SAVED_LIVES_DATA_ID, 3);
                data = DataBridge.TryGetData<byte>(SAVED_LIVES_DATA_ID);
            }
            _managed = Object.Instantiate
            (
                _prefab,
                GameObject.FindGameObjectWithTag("GUI")
                .transform.GetChild((int)RenderingLayer.LAYER2)
            );

            _currentLives = data.EncodedData;
        }
        private void OnLevelFinish()
        {
            DataBridge.UpdateData<byte>(SAVED_LIVES_DATA_ID, _currentLives);
            FileManager.Instance.SaveData(
                "Resources/" + SAVED_LIVES_DATA_ID, 
                "resource", new byte[1] { _currentLives}, 
                needsReload: false
            );
            Object.Destroy(_managed.gameObject);
        }
        private void OnGameStateEnter(GameState state)
        {
            switch (state)
            {
                case LevelStartState:
                    OnLevelStart();
                    break;
                case LevelFinishState:
                    OnLevelFinish();
                    break;
                default:
                    break;
            }
        }

        private void OnEnable() => GameState.OnEnter += OnGameStateEnter;
        private void OnDisable() => GameState.OnEnter -= OnGameStateEnter;
    }
}
