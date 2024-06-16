using System;
using UnityEngine;

namespace Monoliths.Visualisators
{
    public enum RenderingLayer
    {
        SCREEN = 0,
        LAYER1 = 1,
        LAYER2 = 2,
        LAYER3 = 3
    }

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

        protected virtual void Update()
        {
            if (!_autoDisplay) return;

            try
            {
                var data = DataBridge.TryGetData<DataPacket>(_dataID);
                if (!data.IsEmpty)
                {
                    Display(data.EncodedData);
                    DataBridge.MarkUpdateProcessed<DataPacket>(_dataID);
                }
            }
            catch (InvalidCastException)
            {
                if (IsActive)
                {
                    IsActive = false;
                    _status = $"Stored data was not of type Data<{typeof(DataPacket)}>.";
                }
            }
        }

        protected abstract void Display(DataPacket data);
    }
}