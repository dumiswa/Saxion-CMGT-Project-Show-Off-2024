namespace Monoliths.Visualisators
{
    public struct PopUpData
    {
        public PopUpStackPacket.PopUpTypes Type;
        public string AssetName;
        private object _encapsulatedData;

        public PopUpData(PopUpStackPacket.PopUpTypes type, string assetName, object data = null)
        {
            Type = type;
            AssetName = assetName;
            _encapsulatedData = data;
        }
        public PopUpData(bool _ = false)
        {
            Type = PopUpStackPacket.PopUpTypes.PRESET;
            AssetName = "";
            _encapsulatedData = null;
        }
        public readonly T GetData<T>() => (T)_encapsulatedData;
    }
}