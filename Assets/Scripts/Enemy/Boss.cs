using Monoliths.Mechanisms;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public static Boss Instance;

    [Serializable]
    public class HPEvent
    {
        public byte HPThreshold;
        public Actuator Actuator;

        public bool Deactivated;
    }

    [Serializable]
    public struct Attack
    {
        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private float _preparationTime;

        public bool SpawnPredicate(float timestamp) =>
            timestamp >= _preparationTime;

        public void Spawn(Transform transform) 
            => Instantiate(_prefab, transform);
    }

    [SerializeField]
    private byte _HP;
    [SerializeField]
    private List<HPEvent> _events;
    [Space(10)]
    [SerializeField]
    private List<Attack> _attacks = new();

    private float _counter;
    private Attack _current;

    private Animator _animator;

    private void Awake() => Instance = this;
    private void Start()
    {
        _animator = transform.Find("Display").GetComponent<Animator>();
        _current = _attacks[UnityEngine.Random.Range(0, _attacks.Count)];
    }
    private void Update()
    {
        _counter += Time.deltaTime;
        if (_current.SpawnPredicate(_counter))
        {
            _animator.SetTrigger("Attack");
            _current.Spawn(transform);
            _counter = 0;
            _current = _attacks[UnityEngine.Random.Range(0, _attacks.Count)];
        }
    }

    public void DealDamage()
    {
        _animator.SetTrigger("TakeDamage");
        _HP--;
        if (_HP == 0)
            DataBridge.UpdateData(LevelProgressObserver.BOSS_LEVEL_FINISHED_ID, true);
        foreach (var hpEvent in _events)
        {
            if (hpEvent.Deactivated)
                continue;

            if (_HP <= hpEvent.HPThreshold)
            {
                hpEvent.Deactivated = true;
                hpEvent.Actuator.Invoke();
            }
        }
    }
}
