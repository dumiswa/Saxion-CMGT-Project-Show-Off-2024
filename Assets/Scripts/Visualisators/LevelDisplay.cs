
using UnityEngine;
using UnityEngine.UI;


public class LevelDisplay : MonoBehaviour
{
    [SerializeField]
    private Image _snowflakes;

    [SerializeField]
    private Sprite[] _possible;
    
    public void SetSnowflakes(int count)
    {
        if (_possible == null || _possible.Length <= count)
            return;

        _snowflakes.sprite = _possible[count];   
    }
}
