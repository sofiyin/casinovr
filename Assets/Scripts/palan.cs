using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class LeverSpinTrigger : MonoBehaviour
{
    [SerializeField] private XRLever lever;
    [SerializeField] private SlotMachineController slotMachine;

    private bool hasTriggered = false;

    private void Update()
    {
        if (lever.value && !hasTriggered)
        {
            hasTriggered = true;
            slotMachine.TrySpin();
        }

        if (!lever.value)
        {
            hasTriggered = false;
        }
    }
}