using System;
using System.Collections.Generic;
using System.Reflection;

namespace Monoliths
{
    public class StateRegistrar : Monolith
    {
        private readonly static Dictionary<Type, GameState> _registeredStates = new();
        public static GameState Get(Type type)
            => _registeredStates[type];

        public override void Defaults()
        {
            base.Defaults();
            _priority = 100;
        }

        public override bool Init()
        {

#if UNITY_EDITOR
            SaveMaster.ResetSaveData();
            GameStateMachine.Instance = new();
#endif

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type gameStateType = typeof(GameState);

            int faultyTypes = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (!gameStateType.IsAssignableFrom(type) || type.IsAbstract)
                    continue;

                if (!TryCreateAndRegister(type))
                    faultyTypes++;
            }

            GameStateMachine.Start();

            if (faultyTypes == 0)
                return base.Init();
            else
            {
                IsActive = true;
                _status = $"Initiated with problems, couldn't register {faultyTypes} states";
                return true;
            }

        }

        public bool TryCreateAndRegister(Type type)
        {
            try
            {
                _registeredStates.Add(type, (GameState)Activator.CreateInstance(type));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}