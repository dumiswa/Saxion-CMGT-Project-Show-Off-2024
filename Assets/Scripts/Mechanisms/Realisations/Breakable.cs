using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class Breakable : Actuator
    {
        private GameObject _caller;
        private void Start()
        {
            Locked = true;
        }
        public override void Interact(GameObject caller)
        {
            if (Locked)
                return;

            DataBridge.TryGetData<Invizibilo>(Invizibilo.INVIZIBILO_ACCOMPANING).EncodedData.FinalizeAct();
            DataBridge.UpdateData<Invizibilo>(Invizibilo.INVIZIBILO_ACCOMPANING, null);

            _caller = caller;
            Invoke();
        }
        public override void Invoke()
        {
            Destroy(gameObject);
        }

        private void Update()
        {
            var data = DataBridge.TryGetData<Invizibilo>(Invizibilo.INVIZIBILO_ACCOMPANING);
            if (data.IsEmpty)
                return;

            Locked = data.EncodedData is null;
        }
    }
}
