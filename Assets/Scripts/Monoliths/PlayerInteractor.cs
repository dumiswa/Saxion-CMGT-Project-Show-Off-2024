using UnityEngine;
using UnityEngine.InputSystem;

namespace Monoliths.Player
{
    public class PlayerInteractor : Monolith
    {
        private GameObject _player;
        private LayerMask _interactableLayer;
        private PlayerProfile _controls;

        public override bool Init()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
            {
                _status = "Couldn't Find Player";
                return false;
            }

            _interactableLayer = 1 << LayerMask.NameToLayer("Interactable");

            return base.Init();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            RaycastHit hit;
            if (Physics.Raycast(_player.transform.position, _player.transform.forward, out hit, 2f, _interactableLayer))
            {
                InteractWithObject(hit.collider.gameObject);
            }
        }

        public void InteractWithObject(GameObject obj)
        {
            //Interaction with obj logic
        }
    }
}

