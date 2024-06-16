using System;

namespace Monoliths.Visualisators
{
    [Serializable]
    public struct PopUpData
    {
        public PopUpStackPacket.PopUpTypes Type;
        public string AssetName;
        private object _encapsulatedData;

        private static float _prevIID = 0;
        public float InstanceID;

        public PopUpData(PopUpStackPacket.PopUpTypes type, string assetName, object data = null)
        {
            Type = type;
            AssetName = assetName;
            _encapsulatedData = data;

            _prevIID++;
            InstanceID = _prevIID;
        }
        public PopUpData(bool _ = false)
        {
            Type = PopUpStackPacket.PopUpTypes.PRESET;
            AssetName = "";
            _encapsulatedData = null;

            _prevIID++;
            InstanceID = _prevIID;
        }
        public void SetEncapsulatedData(object data) => _encapsulatedData = data;
        public readonly T GetData<T>() => (T)_encapsulatedData;
    }
}