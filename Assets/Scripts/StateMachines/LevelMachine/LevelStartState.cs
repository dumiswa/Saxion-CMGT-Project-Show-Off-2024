using Monoliths.Player;
using UnityEngine;

public class LevelStartState : LevelSubState
{
    public override void Enter()
    {
        DataBridge.UpdateData(PlayerMovement.MOVEMENT_ENABLED_DATA_ID, false);
        var player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if(player is null)
            return;

        player.position = Vector2.zero;

        //PLAY ANIM
        Debug.Log("Level Started, animation finished, enabling movement");
        (GameStateMachine.Instance.Current as LevelState).SubStateMachine.Next<MidLevelState>();
        
        base.Enter();
    }
}
