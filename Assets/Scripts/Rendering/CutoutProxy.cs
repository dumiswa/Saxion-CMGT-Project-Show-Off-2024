using Monoliths;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutoutProxy : Monolith
{
    private Transform _target;
    private LayerMask _layerMask;

    private Camera _camera;

    private Material[] _prevMaterialBuffer;
    private HashSet<Material> _currentMaterialBuffer;
    private List<Material> _fallOffBuffer;

    public override bool Init()
    {
        _fallOffBuffer = new();
        _camera = Camera.main;
        if (_camera is null)
        {
            _status = "Couldn't Find Camera";
            return false;
        }
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        if (_target is null)
        {
            _status = "Couldn't Find Player";
            return false;
        }
        _layerMask = LayerMask.NameToLayer("SeeThrough");
        return base.Init();
    }

    private void LateUpdate()
    {
        Vector2 cutoutPosition = _camera.WorldToViewportPoint(_target.position);
        cutoutPosition.y /= (Screen.width / Screen.height);

        Vector3 offset = _target.position - _camera.transform.position;
        RaycastHit[] hits = Physics.RaycastAll(_camera.transform.position, offset, offset.magnitude);

        _currentMaterialBuffer = new();

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.layer != _layerMask)
                continue;

            Material[] materials = hits[i].transform.GetComponent<Renderer>().materials;
            for (int j = 0; j < materials.Length; j++)
            {
                if (materials[j].shader.name == "Shader Graphs/SeeThrough")
                {
                    _currentMaterialBuffer.Add(materials[j]);
                    materials[j].SetVector("_CutoutPosition", cutoutPosition);
                    var opacity = materials[j].GetFloat("_MinimumOpacity");

                    if(opacity > 0.8f)
                        materials[j].SetFloat("_MinimumOpacity", opacity - 0.015f);
                }
            }
        }

        if (_prevMaterialBuffer is not null)
        {
            foreach (var material in _prevMaterialBuffer)
            {
                if (!_currentMaterialBuffer.Contains(material))
                    _fallOffBuffer.Add(material);
            }
        }
        _prevMaterialBuffer = _currentMaterialBuffer.ToArray();

        List<Material> finished = new();
        foreach (var material in _fallOffBuffer)
        {
            if (_currentMaterialBuffer.Contains(material))
                finished.Add(material);

            var opacity = material.GetFloat("_MinimumOpacity");

            if (opacity < 2f)
                material.SetFloat("_MinimumOpacity", opacity + 0.015f);
            else
                finished.Add(material);
        }
        foreach (var material in finished)
            _fallOffBuffer.Remove(material);
    }
}
