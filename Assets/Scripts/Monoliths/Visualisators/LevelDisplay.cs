using System;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    public readonly static Action<LevelDisplay> OnLevelSelected;
    [HideInInspector]
    public int Index;

    public LevelResult LevelInfo;

    public void Select() => OnLevelSelected?.Invoke(this);
    public void Clear() => Destroy(gameObject);
}
