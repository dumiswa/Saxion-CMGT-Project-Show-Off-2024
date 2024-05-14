using Monoliths;
using Monoliths.Mechanisms;
using System;
using UnityEngine;

public class LevelProgressObserver : Monolith
{
    private GameObject[] _stars;
    private GameObject[] _levelTargets;

    public static event Action OnLevelShouldFinalize;

    public override bool Init()
    {
        base.Init();

        _stars = GameObject.FindGameObjectsWithTag("Star");
        _levelTargets = GameObject.FindGameObjectsWithTag("LevelTargets");

        foreach (var star in _stars)
        {
            var starComponent = star.GetComponent<Collectable>(); 
            if (starComponent == null)
            {
                starComponent = star.AddComponent<Collectable>();
            }
            starComponent.OnCollison += HandleStarAcquired;
        }
                
        foreach (var target in _levelTargets)
        {
            var targetComponent = target.GetComponent<Collectable>(); 
            if (targetComponent == null)
            {
                targetComponent = target.AddComponent<Collectable>();
            }
                targetComponent.OnCollison += HandleTargetAcquired;
        }

        return true;
    }

    private void OnEnable()
    {
        OnLevelShouldFinalize += FinalizeLevel;
    }
    private void OnDisable()
    {
        OnLevelShouldFinalize -= FinalizeLevel;
    }
    private void FinalizeLevel()
    {
        if (!(GameStateMachine.Instance.Current is LevelState levelState)) return;

        levelState.FinalizeLevel();
    }

    private void HandleStarAcquired(GameObject star)
    {
        
        Debug.Log("Star acquired" + star.name);
    }
    private void HandleTargetAcquired(GameObject levelTarget)
    {

        Debug.Log("LevelTarget acquired" + levelTarget.name);
    }
}

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

public class Collectable : OnCollisionActuator
{
    public override void Invoke()
    {
        base.Invoke();
        Destroy(gameObject);
    }
}
