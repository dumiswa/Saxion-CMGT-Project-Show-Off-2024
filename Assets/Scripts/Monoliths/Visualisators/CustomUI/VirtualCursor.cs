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
    private bool _startedHold;
    private bool _stoppedHold;
    private bool _hidden;

    private HashSet<ICustomUIElement> _hoverStack;
    private HashSet<ICustomUIElement> _holdStack;

    public Vector2 GetPosition() => _cursorPosition;

    private void Start()
    {
        Instance = this;

        _hoverStack = new();
        _holdStack = new();

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
    private void StartHold(InputAction.CallbackContext ctx) => _startedHold = true;
    private void StopHold(InputAction.CallbackContext ctx) => _stoppedHold = true;
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

            if (_startedHold)
            {
                element.StartHold();
                _holdStack.Add(element);
            }
            if (_stoppedHold && _holdStack.Contains(element))
                element.StopHold();

            if(_clicked)
                element.Click();

            if (!_hoverStack.Contains(element))
            {
                _hoverStack.Add(element);
                element.StartHover();
            }

            current.Add(element);
        }

        ManageHoverStack(ref current);
        ManageHoldStack(ref current);

        _startedHold = false;
        _stoppedHold = false;
        _clicked = false;
    }
    private void ManageHoldStack(ref HashSet<ICustomUIElement> current)
    {
        List<ICustomUIElement> toRemoveHold = new();
        foreach (var element in _holdStack)
        {
            try
            {
                if (!current.Contains(element))
                {
                    element.StopHover();
                    toRemoveHold.Add(element);
                }
            }
            catch
            {
                toRemoveHold.Add(element);
            }
        }

        foreach (var element in toRemoveHold)
            _holdStack.Remove(element);

    }
    private void ManageHoverStack(ref HashSet<ICustomUIElement> current)
    {
        List<ICustomUIElement> toRemoveHover = new();
        foreach (var element in _hoverStack)
        {
            try
            {
                if (!current.Contains(element))
                {
                    element.StopHover();
                    toRemoveHover.Add(element);
                }
            }
            catch
            {
                toRemoveHover.Add(element);
            }
        }

        foreach (var element in toRemoveHover)
            _hoverStack.Remove(element);
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