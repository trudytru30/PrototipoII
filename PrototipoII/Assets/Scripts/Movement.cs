using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    private void Update()
    {
        gameObject.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
