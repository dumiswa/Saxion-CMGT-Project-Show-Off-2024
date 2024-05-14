using UnityEngine;

namespace Monoliths.Mechanisms
{
    public abstract class Actuator : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string _identifier;
        public string Identifier { get; protected set; }

        public abstract void Invoke();

        public void ChangeIdentifier(string newIdentifier)
        {
            _identifier = newIdentifier;
            Identifier = newIdentifier;
        }
        private void OnEnable()
        {
            Identifier = string.IsNullOrEmpty(_identifier) ? "Unnamed" : _identifier;
            MechanismsObserver.Register(this);
        }
        private void OnDisable() => MechanismsObserver.Remove(Identifier);

        public virtual void Interact(GameObject caller) {}
        public virtual void Collide(GameObject caller) {}
    }
}