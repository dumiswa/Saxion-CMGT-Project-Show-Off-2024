using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VirtualCursor : MonoBehaviour
{
    public static VirtualCursor Instance { get; private set; }

    [SerializeField]
    private float _cursorSpeed = 1000f;

    private RectTransform _cursorTransform;
    private Canvas _canvas;
    private Image _image;

    private Vector2 _cursorPosition;
    private bool _clicked;
    private bool _hold;
    private bool _hidden;

    private HashSet<ICustomUIElement> _hoverStack;

    public Vector2 GetPosition() => _cursorPosition;

    private void Start()
    {
        Instance = this;

        _hoverStack = new();
        _cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
        _cursorTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _canvas = GameObject.FindGameObjectWithTag("GUI").GetComponent<Canvas>();
        UpdateCursor();

        Controls.Profile.Map.FirstContextualButton.performed += Click;
        Controls.Profile.Map.FirstContextualButton.started += StartHold;
        Controls.Profile.Map.FirstContextualButton.canceled += StopHold;
        GameState.OnEnter += OnStateChanged;
    }
    private void OnDisable() 
    { 
        Controls.Profile.Map.FirstContextualButton.performed -= Click;
        Controls.Profile.Map.FirstContextualButton.started -= StartHold;
        Controls.Profile.Map.FirstContextualButton.canceled -= StopHold;
        GameState.OnEnter -= OnStateChanged;
    }
    private void Update()
    {
        var direction = Controls.LeftDirectional + Controls.RightDirectional;
        direction = new(Mathf.Clamp(direction.x, -1, 1), Mathf.Clamp(direction.y, -1, 1));
        _cursorPosition += _cursorSpeed * Time.deltaTime * direction;

        _cursorPosition.x = Mathf.Clamp(_cursorPosition.x, 0, Screen.width);
        _cursorPosition.y = Mathf.Clamp(_cursorPosition.y, 0, Screen.height);

        UpdateCursor();
        RaycastGUI();
    }
    private void UpdateCursor()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, _cursorPosition, _canvas.worldCamera, out var canvasPosition);
        _cursorTransform.anchoredPosition = canvasPosition;
    }
    private void Click(InputAction.CallbackContext ctx) => _clicked = true;
    private void StartHold(InputAction.CallbackContext ctx) => _hold = true;
    private void StopHold(InputAction.CallbackContext ctx) => _hold = false;
    private void RaycastGUI()
    {
        var pointerData = new PointerEventData(EventSystem.current) {position = _cursorPosition,};
        var results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        HashSet<ICustomUIElement> current = new();
        foreach (var result in results)
        {
            result.gameObject.TryGetComponent<ICustomUIElement>(out var element);
            if (element is null)
                continue;

            if (_hold)
                element.Hold();

            if(_clicked)
                element.Click();

            if (!_hoverStack.Contains(element))
            {
                _hoverStack.Add(element);
                element.StartHover();
            }

            current.Add(element);
        }

        List<ICustomUIElement> toRemove = new();
        foreach (var element in _hoverStack)
        {
            if(element is null)
            {
                toRemove.Add(element);
                continue;
            }
            if (!current.Contains(element))
            {
                try
                {
                    element.StopHover();
                }
                finally
                {
                    toRemove.Add(element);
                }
            }
        }

        foreach (var element in toRemove)
            _hoverStack.Remove(element);

        _clicked = false;
    }

    private void OnStateChanged(GameState state)
    {
        if (GameStateMachine.Instance.Current is LevelState)
            HideCursor();
        else
            RevealCursor();
    }

    private void HideCursor()
    {
        _hidden = true;
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
    }

    private void RevealCursor()
    {
        if(_hidden)
            _cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);

        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0.5f);
        _hidden = false;
    }
}