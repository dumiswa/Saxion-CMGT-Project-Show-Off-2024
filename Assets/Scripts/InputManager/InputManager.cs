using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager
{
    private static PlayerProfile _controls;

    static InputManager()
    {
        _controls = new PlayerProfile();
        _controls.Enable();
    }

    public static Vector2 Movement => _controls.PlayerMovementMap.Move.ReadValue<Vector2>();

    public static void RegisterInteractAction(System.Action<InputAction.CallbackContext> callback)
    {
        _controls.PlayerInteractionMap.Interact.performed += callback;
        _controls.PlayerInteractionMap.Interact.Enable();
    }

    public static void UnregisterInteractAction(System.Action<InputAction.CallbackContext> callback)
    {
        _controls.PlayerInteractionMap.Interact.performed -= callback;
        _controls.PlayerInteractionMap.Interact.Disable();
    }

    public static void RegisterMovementAction(System.Action<InputAction.CallbackContext> callback)
    {
        _controls.PlayerMovementMap.Move.performed += callback;
        _controls.PlayerMovementMap.Move.Enable();
    }

    public static void UnregisterMovementAction(System.Action<InputAction.CallbackContext> callback)
    {
        _controls.PlayerMovementMap.Move.performed -= callback;
        _controls.PlayerMovementMap.Move.Disable();
    }
}
