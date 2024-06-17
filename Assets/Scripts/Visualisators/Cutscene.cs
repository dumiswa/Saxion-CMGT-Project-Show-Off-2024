
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Cutscene : MonoBehaviour
{
    public delegate void OnCutsceneFinishedHandler();
    public OnCutsceneFinishedHandler OnCutsceneFinished;

    [SerializeField]
    private float _time;

    private void Start()
    {
        StartCoroutine(Show());
    }

    private IEnumerator Show()
    {
        yield return new WaitForSeconds(_time);

        OnCutsceneFinished?.Invoke();
    }

}