using UnityEngine;

public static class Controls
{
    public static CameraProfile Camera { get; private set; }
    public static PlayerProfile Player { get; private set; }

    public static Vector2 Movement => Player.PlayerMovementMap.Move.ReadValue<Vector2>();

    static Controls()
    {
        Player = new();
        Camera = new();

        Player.Enable();
        Camera.Enable();

        SetAllControlsTo(true);
    }

    public static void SetAllControlsTo(bool state)
    {
        if (state)
        {
            Player.PlayerInteractionMap.Interact.Enable();
            Player.PlayerMovementMap.Move.Enable();

            Camera.CameraRotationMap.Rotate.Enable();
        }
        else
        {
            Player.PlayerInteractionMap.Interact.Disable();
            Player.PlayerMovementMap.Move.Disable();

            Camera.CameraRotationMap.Rotate.Disable();
        }
    }
}
