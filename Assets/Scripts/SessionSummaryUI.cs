using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// ================================================================
///  SessionSummaryUI.cs  |  Casino VR  |  CS2H01 UTEC 2026
///
///  Panel de resumen que aparece cuando el jugador sale.
///  Coloca este script en el Canvas del panel de resumen.
///
///  ESTRUCTURA DE CANVAS RECOMENDADA:
///  └─ SummaryPanel (este script aquí)
///      ├─ Background (Image semitransparente)
///      ├─ TitleText          (TMP)
///      ├─ StartCreditsText   (TMP)
///      ├─ EndCreditsText     (TMP)
///      ├─ NetResultText      (TMP)
///      ├─ TotalSpinsText     (TMP)
///      ├─ WinsText           (TMP)
///      ├─ LossesText         (TMP)
///      ├─ JackpotsText       (TMP)
///      ├─ BiggestWinText     (TMP)
///      ├─ BestStreakText      (TMP)
///      ├─ WorstStreakText     (TMP)
///      ├─ WinRateText        (TMP)
///      ├─ TimePlayedText     (TMP)
///      ├─ BtnPlayAgain       (Button)
///      └─ BtnQuit            (Button)
/// ================================================================

public class SessionSummaryUI : MonoBehaviour
{
    // ── PANEL PRINCIPAL ───────────────────────────────────────────
    [Header("── Panel ──")]
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private CanvasGroup canvasGroup;   // para fade-in
    [SerializeField] private float fadeInDuration = 0.8f;

    // ── TEXTOS DE ESTADÍSTICAS ────────────────────────────────────
    [Header("── Textos de Estadísticas ──")]
    [SerializeField] private TextMeshProUGUI startCreditsText;
    [SerializeField] private TextMeshProUGUI endCreditsText;
    [SerializeField] private TextMeshProUGUI netResultText;
    [SerializeField] private TextMeshProUGUI totalSpinsText;
    [SerializeField] private TextMeshProUGUI winsText;
    [SerializeField] private TextMeshProUGUI lossesText;
    [SerializeField] private TextMeshProUGUI jackpotsText;
    [SerializeField] private TextMeshProUGUI biggestWinText;
    [SerializeField] private TextMeshProUGUI bestStreakText;
    [SerializeField] private TextMeshProUGUI worstStreakText;
    [SerializeField] private TextMeshProUGUI winRateText;
    [SerializeField] private TextMeshProUGUI timePlayedText;

    // ── BOTONES ───────────────────────────────────────────────────
    [Header("── Botones ──")]
    [SerializeField] private Button btnPlayAgain;
    [SerializeField] private Button btnQuit;

    // ── REFERENCIAS ───────────────────────────────────────────────
    [Header("── Referencias ──")]
    [SerializeField] private SlotMachineController slotMachine;

    // ── COLORES ───────────────────────────────────────────────────
    [Header("── Colores ──")]
    [SerializeField] private Color colorGain  = new Color(0.2f, 1f, 0.4f);   // verde
    [SerializeField] private Color colorLoss  = new Color(1f, 0.3f, 0.3f);   // rojo
    [SerializeField] private Color colorNeutral = Color.white;

    // =============================================================
    //  UNITY
    // =============================================================

    private void Awake()
    {
        if (summaryPanel != null)
            summaryPanel.SetActive(false);

        if (btnPlayAgain != null)
            btnPlayAgain.onClick.AddListener(OnPlayAgain);

        if (btnQuit != null)
            btnQuit.onClick.AddListener(OnQuit);
    }

    // =============================================================
    //  MOSTRAR RESUMEN
    //  Llamar desde ExitDoorTrigger o desde cualquier botón de salida
    // =============================================================

    public void ShowSummary()
    {
        if (SessionManager.Instance == null)
        {
            Debug.LogWarning("[SummaryUI] No hay SessionManager en escena.");
            return;
        }

        // Sincronizar créditos finales con la slot machine
        if (slotMachine != null)
            SessionManager.Instance.SyncCredits(slotMachine.GetCredits());

        summaryPanel.SetActive(true);
        PopulateTexts();

        // Fade-in suave
        if (canvasGroup != null)
            StartCoroutine(FadeIn());

        Debug.Log(SessionManager.Instance.GetSummaryDebug());
    }

    // =============================================================
    //  RELLENAR TEXTOS
    // =============================================================

    private void PopulateTexts()
    {
        var s = SessionManager.Instance;
        if (s == null) return;

        // Créditos
        SetText(startCreditsText, $"💰 Fichas iniciales: {s.startingCredits}");
        SetText(endCreditsText,   $"💰 Fichas finales:   {s.currentCredits}");

        // Resultado neto con color
        int net = s.NetResult;
        string netSign = net >= 0 ? "+" : "";
        string netStr  = $"📊 Resultado neto:   {netSign}{net} fichas";
        if (netResultText != null)
        {
            netResultText.text  = netStr;
            netResultText.color = net > 0 ? colorGain : (net < 0 ? colorLoss : colorNeutral);
        }

        // Contadores
        SetText(totalSpinsText, $"🎰 Giros totales:    {s.TotalSpins}");
        SetText(winsText,       $"✅ Victorias:        {s.TotalWins}");
        SetText(lossesText,     $"❌ Derrotas:         {s.TotalLosses}");
        SetText(jackpotsText,   $"🏆 Jackpots:         {s.Jackpots}");

        // Récords
        SetText(biggestWinText, $"🤑 Mayor ganancia:   {s.BiggestWin} fichas");
        SetText(bestStreakText,  $"🔥 Mejor racha:      {s.BestWinStreak} victorias seguidas");
        SetText(worstStreakText, $"💀 Peor racha:       {s.WorstLoseStreak} derrotas seguidas");

        // Porcentaje de victorias
        float winRate = s.TotalSpins > 0 ? (s.TotalWins * 100f / s.TotalSpins) : 0f;
        SetText(winRateText, $"📈 % Victorias:      {winRate:F1}%");

        // Tiempo
        float dur = s.SessionDuration;
        int minutes = (int)(dur / 60f);
        int seconds = (int)(dur % 60f);
        SetText(timePlayedText, $"⏱ Tiempo jugado:    {minutes}m {seconds}s");
    }

    private void SetText(TextMeshProUGUI tmp, string value)
    {
        if (tmp != null) tmp.text = value;
    }

    // =============================================================
    //  FADE IN
    // =============================================================

    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    // =============================================================
    //  BOTONES
    // =============================================================

    private void OnPlayAgain()
    {
        summaryPanel.SetActive(false);
        // Reiniciar sesión con los créditos actuales
        if (SessionManager.Instance != null && slotMachine != null)
            SessionManager.Instance.StartSession(slotMachine.GetCredits());

        Debug.Log("[SummaryUI] Jugador eligió continuar jugando.");
    }

    private void OnQuit()
    {
        Debug.Log("[SummaryUI] Jugador eligió salir.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // =============================================================
    //  API PÚBLICA
    // =============================================================

    public void Hide()
    {
        if (summaryPanel != null)
            summaryPanel.SetActive(false);
    }
}
