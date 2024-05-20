using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCustomUIElement : MonoBehaviour, ICustomUIElement
{
    private Button _button;
    private void Start() 
        => _button = GetComponent<Button>();

    public void Click() =>_button.onClick.Invoke();

    public void StartHover()
    {
        _button.OnPointerEnter(new PointerEventData(EventSystem.current) 
        { position = VirtualCursor.Instance.GetPosition()});
    }

    public void StopHover()
    {
        _button.OnPointerExit(new PointerEventData(EventSystem.current)
        { position = VirtualCursor.Instance.GetPosition() });
    }

    public void Hold() => Click();

}