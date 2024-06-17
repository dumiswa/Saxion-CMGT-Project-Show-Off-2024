using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelSelectionScreen : MonoBehaviour
{
    [SerializeField]
    private LevelDisplay[] _displays;

    [SerializeField]
    private GameObject _displaySet;
    [SerializeField]
    private GameObject _pointSet;

    private List<LevelInfo> _levels = new();

    private bool _selectionLock;
    private int _index;

    private void Start()
    {
        foreach (var display in _displays)
            display.gameObject.SetActive(false);
    }

    private void Update()
    {
        _displaySet.SetActive(_selectionLock);
        _pointSet.SetActive(!_selectionLock);
    }

    public void AddLevelInfo(LevelInfo level)
    {
        _levels.Add(level);
    }

    public void OpenDisplay(int index)
    {
        if (_selectionLock)
            return;

        _selectionLock = true;

        _index = index + 1;

        _displays[index].gameObject.SetActive(true);
        _displays[index].SetSnowflakes(_levels[_index].CollectedSnowflakes);

        Subscribe();
    }

    private void Subscribe()
    {
        Controls.Profile.Map.FirstContextualButton.performed += StartLevel;
        Controls.Profile.Map.SecondContextualButton.performed += CloseDisplay;
    }

    private void Unsubscribe()
    {
        Controls.Profile.Map.FirstContextualButton.performed -= StartLevel;
        Controls.Profile.Map.SecondContextualButton.performed -= CloseDisplay;
    }

    public void CloseDisplay(InputAction.CallbackContext ctx)
    {
        if (!_selectionLock)
            return;

        Unsubscribe();

        _selectionLock = false;
        foreach (var display in _displays)
            display.gameObject.SetActive(false);
    }

    public void StartLevel(InputAction.CallbackContext ctx)
    {
        if (!_selectionLock || _levels.Count <= _index)
            return;

        Unsubscribe();

        DataBridge.UpdateData(LevelSelectionState.SELECTED_LEVEL_DATA_ID, _levels[_index]);
        GameStateMachine.Instance.Next<LevelState>();
    }
}
