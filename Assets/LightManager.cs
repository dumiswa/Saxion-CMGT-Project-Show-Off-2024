using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private static Transform _player;
    private Light _light;
    private void Start()
    {
        _player ??= GameObject.FindGameObjectWithTag("Player").transform;
        _light = GetComponent<Light>();
    }

    private void Update()
    {
        _light.enabled = Vector3.Distance(_player.position, transform.position) < 16;
    }
}
