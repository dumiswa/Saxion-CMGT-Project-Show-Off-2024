public class GameStateMachine
{
    public static AbstractStateMachine<GameState> Instance;
    public static void Start() => Instance.NextNoExit<MenuState>();
}
