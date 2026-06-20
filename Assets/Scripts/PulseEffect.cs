using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.1f;
        transform.localScale = initialScale * pulse;
    }
}