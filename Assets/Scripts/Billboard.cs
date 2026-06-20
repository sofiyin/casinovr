using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform playerCamera;

    void Update()
    {
        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0);
    }
}