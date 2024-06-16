using Monoliths.Visualisators;
using UnityEngine;

namespace Monoliths.Player
{
    public class PlayerResources : Monolith
    {
        public const string SAVED_LIVES_DATA_ID = "svdplrlives";
        public const string CURRENT_LIVES_DATA_ID = "CurrentPlayerLives";

        private const float INVULNERABILITY_TIME = 0.8f;

        private HPContainer _managed;
        private HPContainer _prefab;

        private Animator _animator;

        private float _invulnerabilityCounter;

        private byte _currentLives
        {
            get => _currentLivesBuffer;
            set
            {
                DataBridge.UpdateData(CURRENT_LIVES_DATA_ID, value);

                if (value < _currentLivesBuffer)
                    _invulnerabilityCounter = 0;

                if (_managed)
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
            _invulnerabilityCounter = INVULNERABILITY_TIME;
            base.Defaults();
        }
        public override bool Init()
        {
            _prefab = Resources.Load<HPContainer>("Prefabs/Visualisators/InGame GUI/HPContainer");
            _animator = GameObject.FindGameObjectWithTag("Player").transform.Find("Display").GetComponent<Animator>();
            return base.Init();
        }

        protected virtual void Update()
        {
            var data = DataBridge.TryGetData<byte>(CURRENT_LIVES_DATA_ID);

            if (_invulnerabilityCounter < INVULNERABILITY_TIME)
                _invulnerabilityCounter += Time.deltaTime;

            if (data.WasUpdated && data.EncodedData >= 0)
            {
                if (_invulnerabilityCounter < INVULNERABILITY_TIME)
                    DataBridge.UpdateData<byte>(CURRENT_LIVES_DATA_ID, _currentLives);
                else
                    _currentLives = data.EncodedData;
            }

            _animator.SetBool("IsTakingDamage", _currentLives > 0 && _invulnerabilityCounter < INVULNERABILITY_TIME);
            _animator.SetBool("IsDead", _currentLives == 0);
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
        }
        private void OnLevelSelectionStart()
        {
            if(_managed != null)
                Object.Destroy(_managed.gameObject);

            _currentLives = 1;
            _invulnerabilityCounter = INVULNERABILITY_TIME;
            _animator.SetTrigger("Revive");
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
                case LevelSelectionState:
                    OnLevelSelectionStart();
                    break;
                default:
                    break;
            }
        }

        private void OnEnable() => GameState.OnEnter += OnGameStateEnter;
        private void OnDisable() => GameState.OnEnter -= OnGameStateEnter;
    }
}
