using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class EnemyPathManager: EditorWindow
{
    private bool _isEditing;
    private WalkingEnemy _current;

    private List<Transform> _waypoints;
    private Transform _managed;

    private WalkingEnemy[] _enemies;

    [MenuItem("Tools/Paths Manager")]
    public static void ShowWindow()
    {
        GetWindow<EnemyPathManager>("Paths Manager");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Refresh"))
            Refresh();
        
        if (_isEditing)
        {
            GUILayout.Label($"Currently editing \"{_current.name}\" with IID: {_current.GetInstanceID()}");
            GUILayout.Label("========================================================================================\n");
            if (GUILayout.Button("Add Waypoint"))
            {
                var prefab = Resources.Load<Transform>("Prefabs/Editor/Waypoint");
                var instance = Instantiate(prefab, _managed);
                
                if (_waypoints.Count > 0) 
                    instance.position = _waypoints[_waypoints.Count - 1].position + new Vector3(0.25f, 0, 0);
                else 
                    instance.position = _current.transform.position;
                
                _waypoints.Add(instance);
                UpdateWaypoints();
            }
            if (GUILayout.Button("Remove Waypoint"))
            {
                if (_waypoints.Count > 0)
                {
                    DestroyImmediate(_waypoints[_waypoints.Count - 1].gameObject);
                    _waypoints.RemoveAt(_waypoints.Count - 1);
                    UpdateWaypoints();
                }
            }
            UpdateWaypoints();
            GUILayout.Label("========================================================================================\n");
            if (GUILayout.Button("Stop Editing"))
            {
                DestroyImmediate(_managed.gameObject);
                _isEditing = false;
                _waypoints = null;
                _current.IsBeingEdited = false;
            }
            GUILayout.Label("========================================================================================\n\n");
        }
        GUILayout.Label("List of all found objects using path:");

        bool toRefresh = false;
        int i = 1;
        GUILayout.Label("--------------------------------");
        foreach (var enemy in _enemies)
        {
            try
            {
                GUILayout.Label($"[{i++}] Name: {enemy.name} // IID: {enemy.GetInstanceID()}");
                if (GUILayout.Button("Select"))
                {
                    Selection.activeGameObject = enemy.gameObject;
                    SceneView.lastActiveSceneView.pivot = enemy.transform.position;
                    SceneView.lastActiveSceneView.Repaint();
                }
                if (GUILayout.Button("Edit Path"))
                {
                    if (_isEditing)
                    {
                        DestroyImmediate(_managed.gameObject);
                        _isEditing = false;
                        _waypoints = null;
                        _current.IsBeingEdited = false;
                    }

                    _current = enemy;
                    _isEditing = true;
                    _waypoints = new();
                    _managed = new GameObject("[DO NOT DESTROY] Managed").transform;

                    _current.IsBeingEdited = true;
                    var prefab = Resources.Load<Transform>("Prefabs/Editor/Waypoint");
                    foreach (var waypoint in enemy.Waypoints)
                    {
                        var instance = Instantiate(prefab, _managed);
                        instance.position = waypoint;
                        _waypoints.Add(instance);
                    }
                }
                GUILayout.Label("Waypoint count: " + enemy.Waypoints.Length);
            }
            catch
            {
                toRefresh = true;
                break;
            }
        }

        if(toRefresh)
            Refresh();
    }

    private void Refresh()
    {
        List<WalkingEnemy> enemies = new();
        var transforms = FindObjectsOfType<Transform>();
        foreach (var transform in transforms)
        {
            transform.gameObject.TryGetComponent(out WalkingEnemy walkingEnemy);
            if (walkingEnemy is null)
                continue;

            else
                enemies.Add(walkingEnemy);
        }
        _enemies = enemies.ToArray();
    }

    private void UpdateWaypoints() 
        => _current.Waypoints = _waypoints.Select(t => t.position).ToArray();
}