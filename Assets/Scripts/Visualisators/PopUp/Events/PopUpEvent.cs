using Monoliths.Player;
using Monoliths.Visualisators;
using System;
using System.Collections;
using UnityEngine;

public class PopUpEvent : OnCollisionActuator
{
    [SerializeField]
    protected PopUpData _data;
    
    [Space(5)]
    [Header("Set to -1 in order to lock infinitely")]
    [SerializeField]
    protected float _cooldown;

    public override void Invoke()
    {
        if (Locked)
            return;

        StartCoroutine(Pipeline());
    }

    private IEnumerator Pipeline()
    {
        var packet = DataBridge.TryGetData<PopUpStackPacket>(PopUpVisualisator.POP_UP_STACK_DATA_ID).EncodedData;
        if (packet.IsLocked())
            yield break;

        Locked = true;
        packet.Add(_data);

        if (_cooldown == -1)
            yield break;

        yield return new WaitForSeconds(_cooldown);
        Locked = false;
    }
}

