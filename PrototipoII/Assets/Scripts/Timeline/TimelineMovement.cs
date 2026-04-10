using System;
using Unity.Cinemachine;
using UnityEngine;

public class TimelineMovement : MonoBehaviour
{
    private void Update()
    {
        this.gameObject.transform.Translate(-Vector3.right * Time.deltaTime);
    }
}
