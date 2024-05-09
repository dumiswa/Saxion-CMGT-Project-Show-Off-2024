using System.Collections.Generic;
using UnityEngine;

namespace Monoliths.Mechanisms
{
    public class MechanismsObserver : Monolith
    {
        private static Dictionary<string, Actuator> _mechanisms;
        private static LayerMask _activeMask;
        public override bool Init()
        {
            _mechanisms = new();
            _activeMask = 1 << LayerMask.NameToLayer("Interactable");

            base.Init();
            _status = $"Successfully Inititated, with active mask \"{_activeMask.ToString()}\"";
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

    public abstract class Actuator : MonoBehaviour
    {
        [SerializeField]
        private string _identifier;
        public string Identifier { get; protected set; }

        public abstract void Invoke();

        public void ChangeIdentifier(string newIdentifier)
        {
            _identifier = newIdentifier;
            Identifier = newIdentifier;
        }
        private void OnEnable()
        {
            Identifier = string.IsNullOrEmpty(_identifier) ? "Unnamed" : _identifier;
            MechanismsObserver.Register(this);
        }
        private void OnDisable() => MechanismsObserver.Remove(Identifier);
    }

    public abstract class Activator : Actuator
    {
        [SerializeField]
        private string[] _actuators;

        public override void Invoke() => MechanismsObserver.Invoke(_actuators);
    }
}