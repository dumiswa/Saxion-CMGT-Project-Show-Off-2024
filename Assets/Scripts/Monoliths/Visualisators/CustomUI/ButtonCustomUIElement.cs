using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCustomUIElement : MonoBehaviour, ICustomUIElement
{
    private Button _button;
    private bool _hold = false;

    private void Start() 
        => _button = GetComponent<Button>();

    private void Update()
    {
        if (_hold)
            Click();
    }

    public void Click() => _button.onClick.Invoke();

    public void StartHover()
    {
        _button.OnPointerEnter(new PointerEventData(EventSystem.current)
        { position = VirtualCursor.Instance.GetPosition() });
    }

    public void StopHover()
    {
        _button.OnPointerExit(new PointerEventData(EventSystem.current)
        { position = VirtualCursor.Instance.GetPosition() });
    }

    public void StartHold() => _hold = true;
    public void StopHold() => _hold = false;
}