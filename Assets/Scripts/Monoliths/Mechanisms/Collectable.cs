using UnityEngine;

public class Collectable : OnCollisionActuator
{
    public override void Invoke()
    {
        Debug.Log("Collected");
        base.Invoke();
        Destroy(gameObject);
    }
}
