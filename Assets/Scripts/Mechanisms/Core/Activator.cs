using UnityEngine;

namespace Monoliths.Mechanisms
{
    public abstract class Activator : Actuator
    {
        [SerializeField]
        private string[] _actuators;

        public override void Invoke() => MechanismsObserver.Invoke(_actuators);
        public override void Interact(GameObject caller) {}
    }
}