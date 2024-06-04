using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class ObjectEnabler : Actuator
    {
        [SerializeField]
        private GameObject _objectToEnable;

        public override void Invoke()
        {
            _objectToEnable.SetActive(true);
            this.gameObject.SetActive(false);   
        }
    }
}
