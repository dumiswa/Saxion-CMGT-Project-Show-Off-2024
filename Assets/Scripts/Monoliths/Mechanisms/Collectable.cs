using UnityEngine;

public class Collectable : OnCollisionActuator
{
    public override void Invoke()
    {
        base.Invoke();
        Destroy(gameObject);
    }
}
