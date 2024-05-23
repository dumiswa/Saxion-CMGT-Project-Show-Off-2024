namespace Monoliths.Player
{
    public class PlayerResources : Monolith
    {
        public const string SAVED_LIVES_DATA_ID = "SavedPlayerLives";
        public const string CURRENT_LIVES_DATA_ID = "CurrentPlayerLives";

        private byte _currentLives
        {
            get => _currentLivesBuffer;
            set
            {
                DataBridge.UpdateData(CURRENT_LIVES_DATA_ID, value);
                _currentLivesBuffer = value;
            }
        }
        private byte _currentLivesBuffer;

        public override void Defaults()
        {
            _currentLives = 0;

            base.Defaults();
        }
        public override bool Init()
        {
            return base.Init();
        }

        private void OnLevelStart()
        {
            var data = DataBridge.TryGetData<byte>(SAVED_LIVES_DATA_ID);
            if (data.IsEmpty || data.EncodedData < 3)
            {
                DataBridge.UpdateData<byte>(SAVED_LIVES_DATA_ID, 3);
                data = DataBridge.TryGetData<byte>(SAVED_LIVES_DATA_ID);
            }

            _currentLives = data.EncodedData;
        }
        private void OnLevelFinish()
            => DataBridge.UpdateData<byte>(SAVED_LIVES_DATA_ID, _currentLives);

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
