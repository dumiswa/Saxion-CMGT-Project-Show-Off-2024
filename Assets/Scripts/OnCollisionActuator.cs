using Monoliths.Mechanisms;
using System;
using UnityEngine;

public class OnCollisionActuator : Actuator
{
    public Action<GameObject> OnCollison;
    public override void Invoke()
    {
        OnCollison?.Invoke(gameObject);
    }

    public override void Collide(GameObject caller)
    {
        Invoke();
    }
}
