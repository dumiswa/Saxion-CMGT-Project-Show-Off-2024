using Monoliths;
using System;
using UnityEngine;

public class LevelProgressObserver : Monolith
{
    private GameObject[] _stars;
    private GameObject[] _levelTargets;

    public override bool Init()
    {
        base.Init();

        _stars = GameObject.FindGameObjectsWithTag("Star");
        _levelTargets = GameObject.FindGameObjectsWithTag("LevelTargets");

        foreach (var star in _stars)
        {
            var starComponent = star.GetComponent<Star>(); 
            if (starComponent != null)
            {
                starComponent.OnStarAcquired += HandleStarAcquired; 
            }
        }

        foreach (var target in _levelTargets)
        {
            var targetComponent = target.GetComponent<LevelTarget>(); 
            if (targetComponent != null)
            {
                targetComponent.OnTargetAcquired += HandleTargetAcquired; 
            }
        }

        return true;
    }

    private void HandleStarAcquired(GameObject star)
    {

        Debug.Log("Star acquired" + star.name);
    }
    private void HandleTargetAcquired(GameObject levelTarget)
    {

        Debug.Log("LevelTarget acquired" + levelTarget.name);
    }

    private void OnDestroy()
    {
        foreach (var star in _stars) 
        {
            var starComponent = star.GetComponent<Star>();
            if (starComponent != null)
                starComponent.OnStarAcquired -= HandleStarAcquired;
        }
        foreach (var levelTarget in _levelTargets)
        {
            var targetComponent = levelTarget.GetComponent<LevelTarget>();
            if (targetComponent != null)
                targetComponent.OnTargetAcquired -= HandleTargetAcquired;
        }
    }
}

public class Star : MonoBehaviour
{
    public Action<GameObject> OnStarAcquired;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnStarAcquired?.Invoke(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}

public class LevelTarget : MonoBehaviour
{
    public Action<GameObject> OnTargetAcquired;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnTargetAcquired?.Invoke(this.gameObject);
    }
}
