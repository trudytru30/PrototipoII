using System;
using Unity.Cinemachine;
using UnityEngine;

public class TimelineMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private void Update()
    {
        this.gameObject.transform.Translate(-Vector3.right * speed* Time.deltaTime);
    }
}
