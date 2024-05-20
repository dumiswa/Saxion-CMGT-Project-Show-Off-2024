using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCustomUIElement : MonoBehaviour, ICustomUIElement
{
    private Button _button;
    private void Start() 
        => _button = GetComponent<Button>();

    public void Click() => _button.onClick.Invoke();

    public bool StartHover()
    {
        try 
        {
            _button.OnPointerEnter(new PointerEventData(EventSystem.current)
            { position = VirtualCursor.Instance.GetPosition() });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool StopHover()
    {
        try
        {
            _button.OnPointerExit(new PointerEventData(EventSystem.current)
            { position = VirtualCursor.Instance.GetPosition() });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Hold() => Click();

}