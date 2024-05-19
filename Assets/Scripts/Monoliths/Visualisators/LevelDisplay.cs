using System;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    public Action<int> OnLevelSelected;
    [HideInInspector]
    public int Index;

    public LevelInfo LevelInfo;

    public void Select() => OnLevelSelected?.Invoke(Index);
    public void Clear() => Destroy(gameObject);
}
