using System.Collections;
using UnityEngine;

namespace Monoliths.Visualisators
{
    public class PopUpTimeBound : ManagedPopUp 
    {
        public float TimeUntilClear;
        protected override void Start()
        {
            base.Start();
            StartCoroutine(WaitToBeCleared());
        }

        private IEnumerator WaitToBeCleared()
        {
            yield return new WaitForSeconds(TimeUntilClear);
            yield return StartRemove();
            RemoveItself();
        }

        protected virtual WaitForSeconds StartRemove() 
            => new(0.0f);
    }
}