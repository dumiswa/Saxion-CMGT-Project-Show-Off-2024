using System;
using System.Collections.Generic;
using System.Reflection;

public class StateRegistrar : Monolith 
{
    private readonly static Dictionary<Type, GameState> _registeredStates = new();
    public static GameState Get(Type type) 
        => _registeredStates[type];
    
    public override bool Init()
    {
        GameStateMachine.Instance = new();

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

        if(faultyTypes == 0)
            return base.Init();
        else 
        {
            IsActive = true;
            MonolithStatus = $"Initiated with problems, couldn't register {faultyTypes} states";
            return true;
        }
    }

    public bool TryCreateAndRegister(Type type)
    {
        try {
            _registeredStates.Add(type, (GameState)Activator.CreateInstance(type));
            return true;
        } catch {
            return false;
        }
    }
}
