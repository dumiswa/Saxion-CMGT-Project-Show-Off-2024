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
            layer.material.SetColor("_Color",
                _renderer.material.GetColor("_Color").a < 0.75f ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1));
        }
    }
}
