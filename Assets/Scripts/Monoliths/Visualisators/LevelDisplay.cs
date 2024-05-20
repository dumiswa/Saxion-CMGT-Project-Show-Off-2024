using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LevelDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _levelName, _levelInfo;

    public Action<int> OnLevelSelected;
    [HideInInspector]
    public int Index;

    public LevelInfo LevelInfo;

    public void Select() => OnLevelSelected?.Invoke(Index);
    public void Clear() => Destroy(gameObject);

    public void Display()
    {
        _levelName.text = LevelInfo.AssetName;
        _levelInfo.text = $"\n" +
            $"<size=75%><b><color=#ababff>Snowflakes</color></b> : [ {LevelInfo.CollectedStars} / {LevelInfo.StarAmount} ]\n" +
            $"<b>{(!LevelInfo.IsCompleted? "<color=#ff6767>Not Complete" : "<color=#67ff67>Complete" )}</b>";
    }

}
