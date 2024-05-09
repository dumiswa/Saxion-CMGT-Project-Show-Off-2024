using System;
using UnityEngine;

namespace Monoliths
{
    [Serializable]
    public class Monolith
    {
        [HideInInspector][SerializeField] private string _name = "None";
        [SerializeField] protected string _status = "Not Initialised";

        protected bool _isActive = false;

        public virtual bool Init()
        {
            _name = GetType().Name;
            _isActive = true;
            _status = "Successfully Initiated";
            return true;
        }
    }
}