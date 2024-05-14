using Monoliths.Mechanisms;
using System.Linq;
using UnityEngine;
namespace Monoliths.Player
{
    public class PlayerInteractor : Monolith
    {
        private GameObject _player;
        private LayerMask _interactableLayer;

        private float _interactionRadius;

        private Collider _closest;

        public void Defaults()
        {
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

            _closest = interactables.OrderBy(result => Vector3.Distance(_player.transform.position, result.transform.position))
                                    .FirstOrDefault();
        }

        public void CollideWithObject(Collider closest)
        {
            if (closest is null)
                return;

            closest.TryGetComponent<IInteractable>(out var interactable);
            
            if (interactable is null)
                return;

            interactable.Collide(_player);
        }
        public void InteractWithObject(Collider closest)
        {
            if (closest is null)
                return;

            closest.TryGetComponent<IInteractable>(out var interactable);

            if (interactable is null)
                return;

            interactable.Interact(_player);
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

