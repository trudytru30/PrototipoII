using UnityEngine;

public struct ShootResult//info del disparo para player y enemigos
{
    public bool hit;
    public RaycastHit hitInfo;//guarda el collider(punto de impacto)
    public Vector3 finalPoint;//cuando no impacta dir* range
    public Vector3 finalDirection;
}
