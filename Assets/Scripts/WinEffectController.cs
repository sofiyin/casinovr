using System.Collections;
using UnityEngine;
using TMPro;

/// ================================================================
///  WinEffectController.cs  |  Casino VR  |  CS2H01 UTEC 2026
///  
///  Maneja la retroalimentación visual/auditiva de ganar o perder.
///  Conectar sus métodos a los eventos OnWin / OnLose del
///  SlotMachineController desde el Inspector.
/// ================================================================

public class WinEffectController : MonoBehaviour
{
    [Header("── Texto de resultado ──")]
    [Tooltip("TextMeshPro que muestra '¡GANASTE!' o 'Inténtalo de nuevo'.")]
    [SerializeField] private TextMeshPro resultText;

    [Tooltip("Segundos que se muestra el texto antes de ocultarse.")]
    [SerializeField] private float displayDuration = 2.5f;

    [Header("── Luces ──")]
    [Tooltip("Luz que parpadea al ganar.")]
    [SerializeField] private Light winLight;
    [SerializeField] private Color winColor  = Color.yellow;
    [SerializeField] private Color loseColor = Color.red;

    [Header("── Partículas ──")]
    [Tooltip("ParticleSystem de monedas (opcional).")]
    [SerializeField] private ParticleSystem coinParticles;

    // ──────────────────────────────────────────────────

    private void Awake()
    {
        if (resultText != null) resultText.gameObject.SetActive(false);
        if (winLight   != null) winLight.enabled = false;
    }

    // ── Llamar desde OnWin del SlotMachineController ──
    public void ShowWin(int amount)
    {
        StopAllCoroutines();
        StartCoroutine(WinRoutine(amount));
    }

    // ── Llamar desde OnLose del SlotMachineController ──
    public void ShowLose()
    {
        StopAllCoroutines();
        StartCoroutine(LoseRoutine());
    }

    // =============================================================

    private IEnumerator WinRoutine(int amount)
    {
        // Texto
        SetText($"¡GANASTE!\n+{amount} fichas", Color.yellow);

        // Luz
        if (winLight != null) { winLight.color = winColor; winLight.enabled = true; }

        // Partículas
        if (coinParticles != null) coinParticles.Play();

        // Parpadeo de luz
        for (int i = 0; i < 6; i++)
        {
            if (winLight != null) winLight.enabled = !winLight.enabled;
            yield return new WaitForSeconds(0.15f);
        }
        if (winLight != null) winLight.enabled = false;

        yield return new WaitForSeconds(displayDuration - 0.9f);
        HideText();
    }

    private IEnumerator LoseRoutine()
    {
        SetText("Inténtalo\nde nuevo", Color.white);

        if (winLight != null) { winLight.color = loseColor; winLight.enabled = true; }

        yield return new WaitForSeconds(0.5f);
        if (winLight != null) winLight.enabled = false;

        yield return new WaitForSeconds(displayDuration - 0.5f);
        HideText();
    }

    private void SetText(string msg, Color color)
    {
        if (resultText == null) return;
        resultText.text  = msg;
        resultText.color = color;
        resultText.gameObject.SetActive(true);
    }

    private void HideText()
    {
        if (resultText != null) resultText.gameObject.SetActive(false);
    }
}
