using UnityEngine;

namespace Monoliths.Visualisators
{
    public class PopUp : MonoBehaviour
    {
        private RectTransform _rect;

        private Transform _player;
        public Transform WorldPoint;
        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _rect = GetComponent<RectTransform>();
        }
        private void Update()
        {
            if (WorldPoint is not null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle
                (
                    _rect.parent as RectTransform,
                    Camera.main.WorldToScreenPoint((WorldPoint.position + _player.position) * 0.5f),
                    null,
                    out Vector2 canvasPosition
                );
                _rect.localPosition = canvasPosition;
            }
        }
    }
}