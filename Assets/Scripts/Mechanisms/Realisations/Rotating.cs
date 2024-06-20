using UnityEngine;

public class Rotating : MonoBehaviour
{
    private void Update()
    {
        transform.eulerAngles += Vector3.up * Time.deltaTime * 45f;       
    }
}