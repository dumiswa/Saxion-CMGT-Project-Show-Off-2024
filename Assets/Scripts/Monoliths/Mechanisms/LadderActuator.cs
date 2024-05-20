using Monoliths.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class LadderActuator : OnCollisionActuator
    {
        public override void Invoke()
        {
            base.Invoke();
            Debug.Log("Ladder interaction triggered");
            DataBridge.UpdateData(PlayerMovement.LADDER_INTERACTED_DATA_ID, true);
        }
    }
}
