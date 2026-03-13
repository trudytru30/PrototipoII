using Unity.Mathematics;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float speed = 500f;
    [SerializeField] private float rotationSpeed = 500f;
    public void Move(Vector3 direction)
    {
        Vector3 movement = transform.position + direction * speed * Time.deltaTime;
        transform.position = movement;
    }

    public void Rotate(quaternion rotation)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}