using System.Collections.Generic;

namespace Monoliths.Visualisators
{
    public class PopUpStackPacket
    {
        public enum PopUpTypes
        {
            IN_WORLD,
            TEXTBOX_TUTORIAL,
            PRESET
        }

        public Dictionary<ushort, PopUpData> PopUps = new();
        private Queue<ushort> _releasedIds = new();

        private bool _locked = true;

        public bool IsLocked() => _locked;
        public void SetLock(bool state) 
            => _locked = state;

        public ushort Add(PopUpData data)
        {
            if (_locked)
                return ushort.MaxValue;

            ushort id = (ushort)(PopUps.Count + 1);
            if (_releasedIds.Count > 0)
                id = _releasedIds.Dequeue();

            PopUps.Add(id, data);
            return id;
        }

        public void Remove(ushort id) 
            => PopUps.Remove(id);

        public void ReleaseId(ushort id) 
            => _releasedIds.Enqueue(id);

        public bool Contains(PopUpData data) 
          => PopUps.ContainsValue(data);

        public bool Contains(ushort id)
          => PopUps.ContainsKey(id);
    }
}