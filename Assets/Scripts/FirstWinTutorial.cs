using UnityEngine;
using TMPro;
using System.Collections;

public class FirstWinTutorial : MonoBehaviour
{
    [SerializeField] private SlotMachineController slotMachine;
    [SerializeField] private GameObject tutorialText;

    private bool shown = false;

    private void OnEnable()
    {
        slotMachine.OnWin.AddListener(ShowTutorial);
    }

    private void OnDisable()
    {
        slotMachine.OnWin.RemoveListener(ShowTutorial);
    }

    private void ShowTutorial(int payout)
    {
        if (shown)
            return;

        shown = true;

        tutorialText.SetActive(true);

        StartCoroutine(HideAfterSeconds());
    }

    private IEnumerator HideAfterSeconds()
    {
        yield return new WaitForSeconds(3f);

        tutorialText.SetActive(false);
    }
}