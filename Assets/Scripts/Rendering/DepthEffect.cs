using UnityEngine;

public class DepthEffect : MonoBehaviour
{
    private SpriteRenderer _renderer;

    [SerializeField]
    private SpriteRenderer[] _depthStack;
    private void Start()
    {
        if (_depthStack is null)
            enabled = false;

        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var sprite = _renderer.sprite;
        foreach (var layer in _depthStack)
        {
            if (layer.sprite == sprite)
                continue;

            layer.sprite = sprite;
        }
    }
}
