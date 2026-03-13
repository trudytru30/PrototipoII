using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float attackOrigin;
    [SerializeField] private float attackTarget;
    [SerializeField] private float attackDuration;

    public float Origin => attackOrigin;
    public float Target => attackTarget;
    public float Duration => attackDuration;
}