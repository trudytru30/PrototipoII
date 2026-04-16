using UnityEngine;

public class TimelineClock : MonoBehaviour
{
    [SerializeField] private float totalDuration = 60f;

    public float CurrentTime { get; private set; }
    public float TotalDuration => totalDuration;

    private void Update()
    {
        CurrentTime += Time.deltaTime;
        if (CurrentTime > totalDuration)
            CurrentTime = totalDuration;
    }
}
