using Monoliths.Visualisators;
using System.Collections;
using UnityEngine;

public class QueuedPopUpEvents : OnCollisionActuator
{
    [SerializeField]
    protected PopUpData[] _datas;

    [Space(5)]
    [SerializeField]
    private float _timeOffset = 1f;
    [SerializeField]
    private float _defaultLength = 4f;


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
        foreach (var data in _datas) 
        {
            float awaitTime = _defaultLength;
            packet.Add(data);
            try
            {
                var timeData = Resources.Load<PopUpTimeBound>("Prefabs/PopUps/" + data.AssetName);
                awaitTime = timeData.TimeUntilClear;
            }
            catch {}
            yield return new WaitForSeconds(awaitTime + _timeOffset);
        }

        if (_cooldown == -1)
            yield break;

        yield return new WaitForSeconds(_cooldown);
        Locked = false;
    }
}

