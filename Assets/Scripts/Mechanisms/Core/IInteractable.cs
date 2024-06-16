using UnityEngine;

namespace Monoliths.Mechanisms
{
    public interface IInteractable
    {
        public void Interact(GameObject caller);
        public void Collide(GameObject caller);
    }
}