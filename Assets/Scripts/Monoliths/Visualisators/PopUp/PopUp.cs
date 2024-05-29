using UnityEngine;

namespace Monoliths.Visualisators
{
    public class PopUp : MonoBehaviour
    {
        protected RectTransform _rect;
        protected virtual void Start() 
            => _rect = GetComponent<RectTransform>();
    }
}