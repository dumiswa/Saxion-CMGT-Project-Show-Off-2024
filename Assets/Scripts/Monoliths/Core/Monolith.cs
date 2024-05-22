using System;
using UnityEngine;

namespace Monoliths
{
    [Serializable]
    public class Monolith
    {
        [HideInInspector][SerializeField] private string _name = "None";
        [SerializeField] protected string _status = "Not Initialised";
        [SerializeField] protected float _priority = 0;
        [SerializeField] public bool IsActive = false;
        
        public float GetPriority() 
            => _priority;

        public virtual void Defaults() 
        {
            _priority = 0;
        }
        public virtual bool Init()
        {
            _name = GetType().Name;
            IsActive = true;
            _status = "Successfully Initiated";
            return true;
        }

        public bool SetActive(bool state)
            => IsActive = state;
    }
}