using TMPro;
using UnityEngine;

public class SnowflakeContainer : MonoBehaviour
{
    private Animator _prefab;
    private Animator[] _bars;

    private void Awake() 
        => _prefab = Resources.Load<Animator>("Prefabs/Visualisators/InGame GUI/Snowflakes/Bar");

    public Animator[] ScaleSnowflakesTo(byte amount)
    {
        if(_bars != null)
            foreach (var bar in _bars)
                Destroy(bar.gameObject);

        _bars = new Animator[amount];
        for (int i = 0; i < amount; i++)
            _bars[i] = Instantiate(_prefab, transform);

        return _bars;
    }
}
