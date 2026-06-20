using UnityEngine;

public class MachineProximityAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip welcomeClip;

    private bool hasPlayed = false;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entró: " + other.name + " | Tag: " + other.tag);

        if (hasPlayed) return;

        if (other.name == "XR Origin")  // ← comparar por nombre
        {
            audioSource.PlayOneShot(welcomeClip);
            hasPlayed = true;
        }
    }
}
