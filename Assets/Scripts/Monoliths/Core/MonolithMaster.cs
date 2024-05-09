using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Monoliths
{
    public class MonolithMaster : MonoBehaviour
    {
        public static MonolithMaster Instance { get; private set; }

        [SerializeField]
        private List<Monolith> _monoliths = new();

        private delegate void OnPlayerLoop();

        private OnPlayerLoop _onAwake;
        private OnPlayerLoop _onStart;
        private OnPlayerLoop _onUpdate;
        private OnPlayerLoop _onLateUpdate;
        private OnPlayerLoop _onFixedUpdate;
        private OnPlayerLoop _onEnable;
        private OnPlayerLoop _onDisable;

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
                if (!typeof(Monolith).IsAssignableFrom(type) || type.IsAbstract || type == typeof(Monolith))
                    continue;
                try
                {
                    var monolith = (Monolith)Activator.CreateInstance(type);
                    monolith.Init();
                    SubscribeToPlayerLoop(monolith);

                    _monoliths.Add(monolith);
                }
                catch (Exception ex)
                {
                    Debug.Log($"Couldn't Init \"{type.Name}\" due to {ex.Message}");
                    continue;
                }
            }
            DontDestroyOnLoad(this);

            _onEnable?.Invoke();
            _onAwake?.Invoke();
        }

        private void SubscribeToPlayerLoop(Monolith monolith)
        {
            foreach (MethodInfo info in monolith.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.NonPublic |
                BindingFlags.Public | BindingFlags.FlattenHierarchy))
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
                        _onStart += action;
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
                    case "OnEnable":
                        _onEnable += action;
                        break;
                    case "OnDisable":
                        _onDisable += action;
                        break;
                    default:
                        break;
                }
            }
        }

        private void Start() => _onStart?.Invoke();
        private void Update() => _onUpdate?.Invoke();
        private void LateUpdate() => _onLateUpdate?.Invoke();
        private void FixedUpdate() => _onFixedUpdate?.Invoke();

        private void OnEnable() => _onEnable?.Invoke();
        private void OnDisable() => _onDisable?.Invoke();
    }
}