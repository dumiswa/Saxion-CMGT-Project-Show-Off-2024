using System;
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Monoliths
{
    public class MonolithMaster : MonoBehaviour
    {
        public static MonolithMaster Instance { get; private set; }

        [SerializeField]
        private List<Monolith> _monoliths = new();
        public Dictionary<Type, Monolith> Monoliths = new();

        public delegate void OnPlayerLoop();

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

            List<Monolith> priorityBuffer = new();

            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if (!typeof(Monolith).IsAssignableFrom(type) || type.IsAbstract || type == typeof(Monolith))
                    continue;
                try
                {
                    var monolith = (Monolith)Activator.CreateInstance(type);
                    monolith.Defaults();

                    priorityBuffer.Add(monolith);
                }
                catch
                {
                    Debug.Log($"Couldn't Create Instance of \"{type.Name}\"");
                    continue;
                }
            }
            DontDestroyOnLoad(this);

            priorityBuffer.Sort((a, b) => b.GetPriority().CompareTo(a.GetPriority()));

            foreach (var monolith in priorityBuffer)
            {
                try
                {
                    monolith.Init();

                    SubscribeToPlayerLoop(monolith);
                    _monoliths.Add(monolith);
                }
                catch (Exception ex)
                {
                    Debug.Log($"Couldn't Init \"{monolith.GetType().Name}\" due to {ex.Message}");
                    continue;
                }
            }

            foreach (var monolith in _monoliths)
                Monoliths.Add(monolith.GetType(), monolith);

            _onAwake?.Invoke();
            Instance = this;
        }

        private void SubscribeToPlayerLoop(Monolith monolith)
        {
            foreach (MethodInfo info in monolith.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.NonPublic |
                BindingFlags.Public | BindingFlags.FlattenHierarchy))
            {
                if (info.ReturnType != typeof(void) || info.GetParameters().Length != 0)
                    continue;

                OnPlayerLoop action = () => 
                {
                    if (monolith.IsActive)
                        info.Invoke(monolith, null);
                };
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

        public void UnsafeSubscribeUpdate(OnPlayerLoop action) => _onUpdate += action;
        public void UnsafeUnsubscribeUpdate(OnPlayerLoop action) => _onUpdate -= action;

        private void Start() => _onStart?.Invoke();
        private void Update() => _onUpdate?.Invoke();
        private void LateUpdate() => _onLateUpdate?.Invoke();
        private void FixedUpdate() => _onFixedUpdate?.Invoke();

        private void OnEnable() => _onEnable?.Invoke();
        private void OnDisable() => _onDisable?.Invoke();

        public void RunCoPlayerLoop(IEnumerator coroutine) => StartCoroutine(coroutine);
    }
}
