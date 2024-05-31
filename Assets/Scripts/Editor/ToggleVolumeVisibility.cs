using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;

public class ToggleVolumeVisibility : EditorWindow
{
    private static bool _isActive = false;

    [MenuItem("Tools/Volume Visibility")]
    public static void ShowWindow()
    {
        GetWindow<ToggleVolumeVisibility>("Volume Visibility");
    }

    private void OnGUI()
    {
        var materials = Resources.FindObjectsOfTypeAll<Material>().Where(m => m.shader.name == "Custom/VolumeDisplay");
        foreach (var material in materials)
        {
            if (material.name == "Custom/VolumeDisplay")
                continue;

            GUILayout.Label(material.name);
            if (GUILayout.Button("Toggle Visible"))
            {
                _isActive = true;
                Toggle(material);
            }

            if (GUILayout.Button("Toggle Invisible"))
            {
                _isActive = false;
                Toggle(material);
            }
        }
    }

    private void Toggle(Material material)
    {
        material.SetFloat("_Active", _isActive ? 1.0f : 0.0f);
        EditorUtility.SetDirty(material);
    }
}
