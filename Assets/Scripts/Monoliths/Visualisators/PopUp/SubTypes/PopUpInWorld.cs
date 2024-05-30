using UnityEngine;

namespace Monoliths.Visualisators
{
    public class PopUpInWorld : PopUp 
    {
        private Transform _player;
        public Transform WorldPoint;
        protected override void Start()
        {
            base.Start();
           _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        private void Update()
        {
            if (WorldPoint is not null)
            {
                try
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
                catch{}
            }
        }
    }
}