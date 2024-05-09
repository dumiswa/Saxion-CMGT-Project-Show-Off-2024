using UnityEngine;
using UnityEngine.InputSystem;

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

        _controls = new PlayerProfile();
        _controls.PlayerInteractionMap.Interact.performed += OnInteractPerformed;
        _controls.PlayerInteractionMap.Interact.Enable();

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

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
}
