using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class SlotMachineController : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private XRSimpleInteractable button;

    [Header("Reels (optional visual rotation)")]
    [SerializeField] private Transform reel1;
    [SerializeField] private Transform reel2;
    [SerializeField] private Transform reel3;

    [Header("UI")]
    [SerializeField] private TextMeshPro resultText;

    [Header("Settings")]
    [SerializeField] private float spinDuration = 2f;
    [SerializeField] private float spinSpeed = 720f;
    [SerializeField] private int credits = 10;
    [SerializeField] private int betAmount = 1;

    private static readonly string[] symbols = { "🍒", "🍋", "🍊", "🔔", "⭐", "7️⃣", "BAR" };
    private static readonly Dictionary<string, int> payouts = new()
    {
        { "🍒", 2 }, { "🍋", 3 }, { "🍊", 4 }, { "🔔", 5 }, { "⭐", 10 }, { "7️⃣", 20 }, { "BAR", 50 }
    };

    private bool isSpinning = false;

    private void Start()
    {
        if (button != null)
            button.selectEntered.AddListener(OnButtonPressed);

        UpdateDisplay("Press to spin!\nCredits: " + credits);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.selectEntered.RemoveListener(OnButtonPressed);
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (!isSpinning && credits >= betAmount)
            StartCoroutine(Spin());
    }

    private IEnumerator Spin()
    {
        isSpinning = true;
        credits -= betAmount;
        UpdateDisplay("Spinning...");

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            float delta = Time.deltaTime;
            RotateReel(reel1, spinSpeed * delta);
            RotateReel(reel2, spinSpeed * delta * 1.1f);
            RotateReel(reel3, spinSpeed * delta * 0.9f);
            elapsed += delta;
            yield return null;
        }

        string s1 = symbols[Random.Range(0, symbols.Length)];
        string s2 = symbols[Random.Range(0, symbols.Length)];
        string s3 = symbols[Random.Range(0, symbols.Length)];

        bool win = s1 == s2 && s2 == s3;
        if (win)
        {
            int prize = payouts[s1] * betAmount;
            credits += prize;
            UpdateDisplay($"{s1} {s2} {s3}\n🎉 WIN! +{prize} credits\nCredits: {credits}");
        }
        else
        {
            UpdateDisplay($"{s1} {s2} {s3}\nNo win\nCredits: {credits}");
        }

        if (credits <= 0)
            UpdateDisplay("Game Over!\nNo credits left.");

        isSpinning = false;
    }

    private void RotateReel(Transform reel, float amount)
    {
        if (reel != null)
            reel.Rotate(amount, 0f, 0f, Space.Self);
    }

    private void UpdateDisplay(string message)
    {
        if (resultText != null)
            resultText.text = message;
        else
            Debug.Log("[SlotMachine] " + message);
    }
}
