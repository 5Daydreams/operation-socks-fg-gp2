using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraBlend : MonoBehaviour
{
    [SerializeField] private List<CinemachineVirtualCamera> cameras;
    private bool cameraToggle = true;

    void Start()
    {
        SetPrio(1);
    }

    private void SetPrio(int index)
    {
        StartCoroutine(UpdatePrio(index));
    }

    private IEnumerator UpdatePrio(int index)
    {
        foreach (var cam in cameras)
        {
            cam.Priority = 0;
        }
        yield return new WaitForSeconds(0.5f);
        cameras[index].Priority = 1;
    }
#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUI.Button(new Rect(120, 20, 100, 40), "Toggle camera"))
        {
            if (cameraToggle)
            {
                StartCoroutine(UpdatePrio(2));
            }
            else
            {
                StartCoroutine(UpdatePrio(1));
            }
            cameraToggle = !cameraToggle;
        }
    }
#endif
}
