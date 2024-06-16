using System.Collections.Generic;
using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class MechanismsObserver : Monolith
    {
        private static readonly Dictionary<string, Actuator> _mechanisms = new();
        private static LayerMask _activeMask = LayerMask.NameToLayer("Interactable");

        public override void Defaults()
        {
            base.Defaults();
            _priority = -1;
        }
        public override bool Init()
        {
            base.Init();
            _status = $"Successfully Inititated, with active mask \"{LayerMask.LayerToName(_activeMask.value)}\"";
            return true;
        }
        public static void Register(Actuator actuator)
        {
            string initialIdentifier = actuator.Identifier;
            int i = 0;
            while (_mechanisms.ContainsKey(actuator.Identifier))
                actuator.ChangeIdentifier(initialIdentifier + i++.ToString());

            _mechanisms.Add(actuator.Identifier, actuator);
            actuator.gameObject.layer = _activeMask;
        }
        public static void Remove(string identifier)
        {
            if (!_mechanisms.ContainsKey(identifier))
                return;

            _mechanisms.Remove(identifier);
        }
        public static void Invoke(string[] identifiers)
        {
            if (identifiers is null)
                return;

            foreach (var identifier in identifiers)
            {
                _mechanisms.TryGetValue(identifier, out var mechanism);
                mechanism?.Invoke();
            }
        }
    }
}