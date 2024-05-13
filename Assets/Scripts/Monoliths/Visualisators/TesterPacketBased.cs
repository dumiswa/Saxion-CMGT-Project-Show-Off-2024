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
            _text = Object.Instantiate(Resources.Load<TextMeshProUGUI>("Prefabs/Tester"), _gui).GetComponent<TextMeshProUGUI>();
            _text.rectTransform.anchoredPosition = Vector3.zero;
        }
        protected override void Update()
        {
            DataBridge.UpdateData<TesterPacket>(typeof(TesterPacket).Name, new() { text = "Test text" });
            base.Update();
        }
        protected override void Display(TesterPacket data)
        {
            _text.text = data.text;
        }
    }
}