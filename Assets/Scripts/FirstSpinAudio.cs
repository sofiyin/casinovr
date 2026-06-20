using UnityEngine;

public class FirstSpinAudio : MonoBehaviour
{
    [SerializeField] private SlotMachineController slotMachine;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tutorialClip;

    private bool played = false;

    private void OnEnable()
    {
        slotMachine.OnSpinStarted.AddListener(PlayTutorialAudio);
    }

    private void OnDisable()
    {
        slotMachine.OnSpinStarted.RemoveListener(PlayTutorialAudio);
    }

    private void PlayTutorialAudio()
    {
        if (played)
            return;

        played = true;

        audioSource.PlayOneShot(tutorialClip);
    }
}