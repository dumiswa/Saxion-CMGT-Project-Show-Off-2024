using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Serializable]
    public struct Attack
    {
        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private float _preparationTime;

        public bool SpawnPredicate(float timestamp) => 
            timestamp >= _preparationTime;

        public void Spawn() 
            => Instantiate(_prefab);
    }

    [SerializeField]
    private List<Attack> _attacks = new();

    private float _counter;
    private Attack _current;

    private void Start() 
        => _current = _attacks[UnityEngine.Random.Range(0, _attacks.Count)];

    private void Update()
    {
        _counter += Time.deltaTime;
        if (_current.SpawnPredicate(_counter))
        {
            _current.Spawn();
            _counter = 0;
            _current = _attacks[UnityEngine.Random.Range(0, _attacks.Count)];
        }
    }
}
