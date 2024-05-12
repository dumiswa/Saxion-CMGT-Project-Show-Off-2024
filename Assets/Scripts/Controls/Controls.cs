using UnityEngine;

public static class Controls
{
    public static InputProfile Profile { get; private set; }

    public static Vector2 LeftDirectional => Profile.Map.LeftDirectional.ReadValue<Vector2>();
    public static Vector2 RightDirectional => Profile.Map.RightDirectional.ReadValue<Vector2>();

    static Controls()
    {
        Profile = new();

        Profile.Enable();
        SetAllControlsTo(true);
    }

    public static void SetAllControlsTo(bool state)
    {
        if (state)
        {
            Profile.Map.LeftDirectional.Enable();
            Profile.Map.RightDirectional.Enable();

            Profile.Map.FirstContextualButton.Enable();
            Profile.Map.SecondContextualButton.Enable();
        }
        else
        {
            Profile.Map.LeftDirectional.Disable();
            Profile.Map.RightDirectional.Disable();

            Profile.Map.FirstContextualButton.Disable();
            Profile.Map.SecondContextualButton.Disable();
        }
    }
}
