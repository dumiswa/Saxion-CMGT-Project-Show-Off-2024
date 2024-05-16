using System;
using UnityEngine;

namespace Monoliths.Visualisators
{
    public abstract class AbstractVisualisator<DataPacket> : Monolith where DataPacket : new()
    {
        protected Transform _gui { get; private set; }
        protected string _dataID;
        protected bool _autoDisplay;

        public AbstractVisualisator(bool autoDisplay = true)
        {
            _autoDisplay = autoDisplay;
            _dataID = typeof(DataPacket).Name;
        }

        public override bool Init() 
        {
            _gui = GameObject.FindGameObjectWithTag("GUI").transform;
            if(_gui is null)
            {
                _status = "Couldn't find GUI";
                return false;
            }
            return base.Init();
        }

        protected virtual void Start()
        {
            if (!_autoDisplay) return;

            try
            {
                var data = DataBridge.TryGetData<DataPacket>(_dataID);
                if (data != Data<DataPacket>.Empty)
                {
                    Display(data.EncodedData);
                    DataBridge.MarkUpdateProcessed<DataPacket>(_dataID);
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
        protected virtual void Update()
        {
            if (!_autoDisplay) return;

            try
            {
                var data = DataBridge.TryGetData<DataPacket>(_dataID);
                if (data != Data<DataPacket>.Empty && data.WasUpdated)
                {
                    if (!_isActive) 
                        base.Init();

                    Display(data.EncodedData);
                    DataBridge.MarkUpdateProcessed<DataPacket>(_dataID);
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