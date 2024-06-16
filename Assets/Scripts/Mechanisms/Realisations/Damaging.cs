using Monoliths.Player;
using System.Collections;
using UnityEngine;

public class Damaging : OnCollisionActuator
{
    [SerializeField]
    private float _timeTillDestroy;

    private bool _destroyed;
    public override void Invoke()
    {
        if (_destroyed)
            return;

        _destroyed = true;
        base.Invoke();
        DataBridge.UpdateData
        (
            PlayerResources.CURRENT_LIVES_DATA_ID,
            (byte)(DataBridge.TryGetData<byte>(PlayerResources.CURRENT_LIVES_DATA_ID).EncodedData - 1)
        );
        StartCoroutine(WaitTillDestroy());
    }

    private IEnumerator WaitTillDestroy()
    {
        yield return new WaitForSeconds(_timeTillDestroy);
        Destroy(gameObject);
    }
}
