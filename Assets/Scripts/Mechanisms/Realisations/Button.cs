using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class Button : Activator
    {
        public override void Interact(GameObject caller)
        {
            Invoke();
        }          
    }
}