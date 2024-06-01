using System.Collections.Generic;
using UnityEngine;

namespace Monoliths.Player
{
    public struct CharacterAnimatorData
    {
        public Vector3 Motion;
        public Vector3 Derivative;

        public CharacterAnimatorData(bool _ = false)
        {
            Motion = Vector3.zero;
            Derivative = Vector3.zero;
        }
    }
    public class CharacterAnimatorStackPacket
    {
        private Animator _playerSpecific;
        public Dictionary<string, Animator> Rotational;

        public bool AsyncLock;

        public CharacterAnimatorStackPacket()
        {
            Rotational = new();
        }

        public void RegisterPlayerSpecific(Animator animator) 
            => _playerSpecific = animator;  
        public void UnregisterPlayerSpecific() 
            => _playerSpecific = null;    

        public void TryRegisterRotations(Animator animator)
        {
            if (Rotational.ContainsValue(animator))
                return;

            var dataID = GetDataIDFromAnimator(animator);
            DataBridge.UpdateData<CharacterAnimatorData>
            (
                dataID,
                new()
            );
            Rotational.Add(dataID, animator);
        }
        public void UnregisterRotations(Animator animator)
        {
            if (!Rotational.ContainsValue(animator))
                return;

            var dataID = GetDataIDFromAnimator(animator);
            DataBridge.ReleaseData(dataID);
            Rotational.Remove(dataID);
        }

        public void Defaults() 
        {
            _playerSpecific = null;
            Rotational.Clear();
        }

        public static string GetDataIDFromAnimator(Animator animator)
            =>animator.GetHashCode().ToString();
    }
}
