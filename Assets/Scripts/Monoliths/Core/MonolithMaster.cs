using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class MonolithMaster : MonoBehaviour
{
    public static MonolithMaster Instance { get; private set; }

#if UNITY_EDITOR
    [SerializeField]
    private string[] _monolithStatuses;
#endif

    private List<Monolith> _monoliths = new();

    private delegate void OnPlayerLoop();

    private OnPlayerLoop _onAwake;
    private OnPlayerLoop _onStart;
    private OnPlayerLoop _onUpdate;
    private OnPlayerLoop _onLateUpdate;
    private OnPlayerLoop _onFixedUpdate;

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(this);
            return;
        }

        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (!typeof(Monolith).IsAssignableFrom(type) || type.IsAbstract)
                continue;

            try{
                var monolith = (Monolith)Activator.CreateInstance(type);
                monolith.Init();
                SubscribeToPlayerLoop(monolith);

                _monoliths.Add(monolith);
            }catch { 
                continue; 
            }
        }
#if UNITY_EDITOR
        _monolithStatuses = new string[_monoliths.Count];
#endif
        DontDestroyOnLoad(this);

        _onAwake?.Invoke();
    }

    private void SubscribeToPlayerLoop(Monolith monolith)
    {
        foreach (MethodInfo info in monolith.GetType().GetMethods
            (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (info.ReturnType != typeof(void) || info.GetParameters().Length != 0)
                continue;

            OnPlayerLoop action = () => info.Invoke(monolith, null);
            switch (info.Name)
            {
                case "Awake":
                    _onAwake += action;
                    break;
                case "Start":
                    _onStart +=  action;
                    break;
                case "Update":
                    _onUpdate += action;
                    break;
                case "LateUpdate":
                    _onLateUpdate += action;
                    break;
                case "FixedUpdate":
                    _onFixedUpdate += action;
                    break;
                default:
                    break;
            }
        }
    }

    private void Start() => _onStart?.Invoke();
    private void Update()
    {
#if UNITY_EDITOR
        for (int i = 0; i < _monolithStatuses.Length; i++)
            _monolithStatuses[i] = $"[{_monoliths[i].GetType().Name}] {_monoliths[i].MonolithStatus}";
#endif
        _onUpdate?.Invoke();
    }
    private void LateUpdate() => _onLateUpdate?.Invoke();
    private void FixedUpdate() => _onFixedUpdate?.Invoke();
}
