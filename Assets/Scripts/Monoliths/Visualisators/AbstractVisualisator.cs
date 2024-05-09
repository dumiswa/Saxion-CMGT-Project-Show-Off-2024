using System;

namespace Monoliths.Visualisators
{
    public abstract class AbstractVisualisator<DataPacket> : Monolith where DataPacket : new()
    {
        protected readonly string _dataID;
        protected bool _autoDisplay;

        public AbstractVisualisator(bool autoDisplay = true)
        {
            _autoDisplay = autoDisplay;
            _dataID = typeof(DataPacket).Name;
        }

        public override bool Init()
            => base.Init();

        protected void Update()
        {
            if (!_autoDisplay) return;

            try
            {
                var data = DataBridge.TryGetData<DataPacket>(_dataID);
                if (data != Data<DataPacket>.Empty)
                {
                    if (!_isActive) base.Init();
                    Display(data.EncodedData);
                    DataBridge.MarkDataClean<DataPacket>(_dataID);
                }
                else if (_isActive)
                {
                    _isActive = false;
                    _status = $"Couldn't get \"{_dataID}\" data packet";
                }
            }
            catch (InvalidCastException)
            {
                if (_isActive)
                {
                    _isActive = false;
                    _status = $"Stored data was not of type Data<{typeof(DataPacket)}>.";
                }
            }
        }

        protected abstract void Display(DataPacket data);
    }
}