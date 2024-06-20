using UnityEngine;

public class Collectable : OnCollisionActuator
{
    public override void Invoke()
    {
        base.Invoke();
        AudioManager.Instance.PlaySound("Collectable");
        Destroy(gameObject);
    }
}
