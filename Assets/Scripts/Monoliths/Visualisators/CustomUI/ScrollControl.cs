using UnityEngine;
using UnityEngine.UI;

public class ScrollControl : MonoBehaviour
{
    private ScrollRect _scrollRect;

    private void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }
    public void MoveRight()
        => _scrollRect.horizontalScrollbar.value += Time.deltaTime * _scrollRect.scrollSensitivity;

    public void MoveLeft() 
        => _scrollRect.horizontalScrollbar.value -= Time.deltaTime * _scrollRect.scrollSensitivity;
}
