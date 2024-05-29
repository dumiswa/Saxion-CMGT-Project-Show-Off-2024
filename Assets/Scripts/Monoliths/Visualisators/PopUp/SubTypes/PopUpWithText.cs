using TMPro;
using UnityEngine;

namespace Monoliths.Visualisators
{
    public class PopUpWithText : PopUp
    {
        [SerializeField]
        protected TextMeshProUGUI _textDisplay;

        public void SetText(string text) 
            => _textDisplay.text = text;
    }
}