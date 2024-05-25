using Monoliths.Mechanisms;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monoliths.Visualisators
{
    public class PopUpVisualisator : AbstractVisualisator<PopUpStackPacket>
    {
        public const string POP_UP_STACK_DATA_ID = "PopUpStackDataId";
        private readonly Dictionary<ushort, PopUp> _instanced = new();

        public override void Defaults()
        {
            base.Defaults();

            _autoDisplay = true;
            _dataID = POP_UP_STACK_DATA_ID;
            _priority = -1;
        }
        public override bool Init()
        {
            DataBridge.UpdateData(POP_UP_STACK_DATA_ID, new PopUpStackPacket());
            return base.Init();
        }
        protected override void Display(PopUpStackPacket data)
        {
            CleanUp(in data);
            Create(in data);
        }
         
        private void CleanUp(in PopUpStackPacket data)
        {
            List<ushort> toRemove = new();
            foreach (var popUp in _instanced.Keys)
            {
                if (!data.PopUps.ContainsKey(popUp))
                {
                     toRemove.Add(popUp);
                     UnityEngine.Object.Destroy(_instanced[popUp].gameObject);
                }
            }
            foreach (var popUp in toRemove)
                _instanced.Remove(popUp);
        }
        private void Create(in PopUpStackPacket data)
        {
            List<(ushort id, PopUpData data)> toInstantiate = new();
            foreach (var popUp in data.PopUps.Keys)
                if (!_instanced.ContainsKey(popUp))
                    toInstantiate.Add((popUp, data.PopUps[popUp]));

            foreach (var popUp in toInstantiate)
                Instantiate(popUp.id, popUp.data);
        }

        private void Instantiate(ushort id, PopUpData data)
        {
            try
            {
                switch (data.Type)
                {
                    case PopUpStackPacket.PopUpTypes.PRESS_BUTTON_SMALL:
                        var closest = data.GetData<Actuator>();

                        Type interactableType = null;
                        if (closest is not null)
                            interactableType = closest.GetType();

                        if (typeof(OnCollisionActuator).IsAssignableFrom(interactableType))
                            break;

                        var prefab = Resources.Load<PopUp>("Prefabs/PopUps/" + data.AssetName);
                        var instance = UnityEngine.Object.Instantiate(prefab, _gui.GetChild(1));

                        instance.WorldPoint = closest.transform;

                        _instanced.Add(id, instance);
                        break;
                    case PopUpStackPacket.PopUpTypes.TEXTBOX_TUTORIAL:
                        break;
                    case PopUpStackPacket.PopUpTypes.PRESET:
                        break;
                    default:
                        break;
                }
            }
            catch{}
        }
    }
}