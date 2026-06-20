using UnityEngine;

public class HideAfterSeconds : MonoBehaviour
{
    [SerializeField] private float seconds = 3f;

    void Start()
    {
        Destroy(gameObject, seconds);
    }
}