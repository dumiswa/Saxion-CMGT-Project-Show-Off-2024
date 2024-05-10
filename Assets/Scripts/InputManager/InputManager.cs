using UnityEngine;

public static class InputManager
{
    public static PlayerProfile Controls { get; private set; }
    public static Vector2 Movement => Controls.PlayerMovementMap.Move.ReadValue<Vector2>();

    static InputManager()
    {
        Controls = new PlayerProfile();
        Controls.Enable();

        SetAllControlsTo(true);
    }

    public static void SetAllControlsTo(bool state)
    {
        if (state)
        {
            Controls.PlayerInteractionMap.Interact.Enable();
            Controls.PlayerMovementMap.Move.Enable();
        }
        else
        {
            Controls.PlayerInteractionMap.Interact.Disable();
            Controls.PlayerMovementMap.Move.Disable();
        }
    }
}
