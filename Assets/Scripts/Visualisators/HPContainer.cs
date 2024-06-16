using TMPro;
using UnityEngine;

public class HPContainer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _hp;

    public void SetCurrent(byte current)
    {
        if (current <= 0)
            _hp.text = "FELL";
        else
            _hp.text = $"<size=80%>x</size>{current}";
    }
}