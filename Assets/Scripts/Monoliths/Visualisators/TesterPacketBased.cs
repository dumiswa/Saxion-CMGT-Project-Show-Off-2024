using TMPro;
using UnityEngine;

namespace Monoliths.Visualisators
{
    public struct TesterPacket
    {
        public string text;
    }
    public class TesterPacketBased : AbstractVisualisator<TesterPacket>
    {
        TextMeshProUGUI _text;

        private void OnEnable()
        {
            _dataID = "TestDataBridgeUniqueDataIDBasedOnTesterPacket";
        }
        private void Start()
        {
            DataBridge.UpdateData<TesterPacket>(_dataID, new() { text = "" });
            _text = Object.Instantiate(Resources.Load<TextMeshProUGUI>("Prefabs/Tester"), _gui).GetComponent<TextMeshProUGUI>();
            _text.rectTransform.anchoredPosition = Vector3.zero;
        }
        protected override void Display(TesterPacket data)
        {
            _text.text = data.text;
        }
    }
}