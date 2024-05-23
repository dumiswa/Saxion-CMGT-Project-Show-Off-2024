using System.Collections.Generic;

namespace Monoliths.Visualisators
{
    public class PopUpStackPacket
    {
        public enum PopUpTypes
        {
            PRESS_BUTTON_SMALL,
            TEXTBOX_TUTORIAL,
            PRESET
        }

        public Dictionary<ushort, PopUpData> PopUps = new();
        private Queue<ushort> _releasedIds = new();

        public ushort Add(PopUpData data)
        {
            ushort id = (ushort)(PopUps.Count + 1);
            if (_releasedIds.Count > 0)
                id = _releasedIds.Dequeue();

            PopUps.Add(id, data);
            return id;
        }

        public void Remove(ushort id)
        {
            PopUps.Remove(id);
            _releasedIds.Enqueue(id);
        }
    }
}