using Monoliths.Mechanisms;
using Monoliths.Visualisators;
using System.Linq;
using UnityEngine;
namespace Monoliths.Player
{
    public class PlayerInteractor : Monolith
    {
        private GameObject _player;
        private LayerMask _interactableLayer;

        private float _interactionRadius;

        private Actuator _prevClosest;
        private Actuator _closest;

        private ushort _popUpId;

        public override void Defaults()
        {
            base.Defaults();

            _popUpId = ushort.MaxValue;
            _interactionRadius = 0.8f;
        }
        public override bool Init()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
            {
                _status = "Couldn't Find Player";
                return false;
            }

            _interactableLayer = LayerMask.NameToLayer("Interactable");

            return base.Init();
        }

        private void Scan()
        {
            var interactables = Physics.OverlapSphere(_player.transform.position, _interactionRadius)
                                       .Where(result => result.gameObject.layer == _interactableLayer);

            foreach (var interactable in interactables)
                CollideWithObject(interactable);

            var collider = interactables.OrderBy(
                result => Vector3.Distance(
                    _player.transform.position, 
                    result.transform.position
                )).FirstOrDefault();

            if (collider is null)
                _closest = null;
            else
                collider.TryGetComponent(out _closest);       

            if (_prevClosest != _closest)
            {
                UpdateVisualisatorData();
                _prevClosest = _closest;
            }

            if(_closest is not null && _closest.Locked && _popUpId != ushort.MaxValue)
            {           
                DataBridge.TryGetData<PopUpStackPacket>(PopUpVisualisator.POP_UP_STACK_DATA_ID)
                    .EncodedData.Remove(_popUpId);
                _popUpId = ushort.MaxValue;
            }
        }

        private void UpdateVisualisatorData()
        {
            var stack = DataBridge.TryGetData<PopUpStackPacket>(PopUpVisualisator.POP_UP_STACK_DATA_ID);
            
            if (stack.IsEmpty)
                return;

            var idBuffer = _popUpId;

            if (_closest is not null && !_closest.Locked)
            {
                _popUpId = stack.EncodedData.Add(new PopUpData(
                   PopUpStackPacket.PopUpTypes.IN_WORLD,
                   "InteractButtonPopUp",
                   _closest
               ));
            }
            else
                _popUpId = ushort.MaxValue;

            if (idBuffer != ushort.MaxValue)
                stack.EncodedData.Remove(idBuffer);
        }

        public void CollideWithObject(Collider other)
        {
            if (other is null)
                return;

            other.TryGetComponent<IInteractable>(out var interactable);
            
            if (interactable is null)
                return;

            interactable.Collide(_player);
        }
        public void InteractWithObject(Actuator closest)
        {
            if (closest is null)
                return;

            closest.Interact(_player);
        }

        private void Update() => Scan();

        private void OnEnable()
        {
            Defaults();

            Controls.Profile.Map.FirstContextualButton.started += ctx => InteractWithObject(_closest);
        }
        private void OnDisable()
        {
            Defaults();

            Controls.Profile.Map.FirstContextualButton.started -= ctx => InteractWithObject(_closest);
        }
    }
}