using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// ================================================================
///  SlotMachineController.cs  |  Casino VR  |  CS2H01 UTEC 2026
///  Compatible: Unity 6 + XR Interaction Toolkit + Meta Quest 2
///
///  CAMBIOS respecto a la versión anterior:
///   ✓ reelTransforms + reelDisplays + symbolMaterials → ReelStrip[]
///   ✓ SpinReel() eliminado (la animación vive en ReelStrip)
///   ✓ ApplySymbols() eliminado (ReelStrip.SnapToResult lo hace)
///   ✓ Fichas, pagos, UI, audio y eventos: sin cambios
/// ================================================================

public class SlotMachineController : MonoBehaviour
{
    // ── RODILLOS ──────────────────────────────────────────────────
    [Header("── Rodillos ──")]
    [Tooltip("Arrastrar aquí los 4 GameObjects con ReelStrip.cs, en orden izquierda → derecha.")]
    [SerializeField] private ReelStrip[] reels;

    // ── TIMING ────────────────────────────────────────────────────
    [Header("── Timing ──")]
    [Tooltip("Segundos mínimos girando antes de empezar a frenar.")]
    [SerializeField] private float spinDuration = 12.0f;
    [Tooltip("Delay entre la orden de parada de un rodillo y el siguiente (efecto dramático izq→der).")]
    [SerializeField] private float staggerDelay = 0.3f;

    // ── FICHAS (RF-04) ────────────────────────────────────────────
    [Header("── Fichas ──")]
    [SerializeField] private int startingCredits = 100;
    [SerializeField] private int betPerSpin = 10;
    private int _credits;

    // ── TABLA DE PAGOS ────────────────────────────────────────────
    [Header("── Tabla de Pagos ──")]
    [Tooltip("Multiplicador por símbolo.\nEj: BAR×10, Seven×5, Cherry×3, Lemon×2")]
    [SerializeField] private int[] payMultipliers = { 10, 5, 3, 2 };

    // ── UI ────────────────────────────────────────────────────────
    [Header("── UI de Dinero ──")]
    [Tooltip("TextMeshPro 3D que muestra el saldo.")]
    [SerializeField] private TextMeshPro creditsText;
    [Tooltip("TextMeshPro 3D que muestra el resultado del giro.")]
    [SerializeField] private TextMeshPro resultText;
    [Tooltip("Segundos que se muestra el texto de resultado.")]
    [SerializeField] private float resultDisplayTime = 2.5f;

    // ── AUDIO ─────────────────────────────────────────────────────
    [Header("── Audio ──")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxSpin;
    [SerializeField] private AudioClip sfxWin;
    [SerializeField] private AudioClip sfxLose;

    // ── EVENTOS ───────────────────────────────────────────────────
    [Header("── Eventos opcionales ──")]
    public UnityEvent<int> OnWin;
    public UnityEvent OnLose;
    public UnityEvent OnSpinStarted;

    // ── ESTADO INTERNO ────────────────────────────────────────────
    private enum State { Idle, Spinning, ShowResult }
    private State _state = State.Idle;
    private int _spinCount = 0;

    // =============================================================
    //  INICIO
    // =============================================================

    private void Awake()
    {
        _credits = startingCredits;
        UpdateCreditsUI();
        HideResultText();
        CheckSetup();
    }

    // =============================================================
    //  PUNTO DE ENTRADA PÚBLICO
    //  Conectar al SelectEntered del XRSimpleInteractable del botón
    // =============================================================

    public void TrySpin()
    {
        if (_state != State.Idle) return;

        if (_credits < betPerSpin)
        {
            ShowResult("Sin fichas", Color.red);
            return;
        }

        _credits -= betPerSpin;
        UpdateCreditsUI();
        StartCoroutine(SpinRoutine());
    }

    // =============================================================
    //  RUTINA PRINCIPAL
    // =============================================================

    private IEnumerator SpinRoutine()
    {
        _state = State.Spinning;
        OnSpinStarted?.Invoke();
        HideResultText();
        PlaySound(sfxSpin);

        // 1. Calcular resultado ANTES de arrancar los rodillos
        //    (necesario para pasárselo a Stop() en el momento correcto)
        int[] result = GenerateResult();

        // 2. Arrancar todos los rodillos a la vez
        foreach (var reel in reels)
            reel.Spin();

        // 3. Girar el tiempo mínimo indicado
        yield return new WaitForSeconds(spinDuration);

        // 4. Frenar rodillos de izquierda a derecha con stagger
        //    Stop() le dice a cada rodillo qué símbolo mostrar;
        //    el rodillo decelera y alinea solo.
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].Stop(result[i]);

            if (i < reels.Length - 1)
                yield return new WaitForSeconds(staggerDelay);
        }

        // 5. Esperar a que el último rodillo termine de frenar del todo
        Debug.Log($"Reels array: {reels?.Length}");
        yield return new WaitUntil(() => !reels[reels.Length - 1].IsSpinning);

        _state = State.ShowResult;
        yield return new WaitForSeconds(0.25f);

        // 6. Calcular pago
        int payout = CalculatePayout(result);

        if (payout > 0)
        {
            _credits += payout;
            UpdateCreditsUI();
            OnWin?.Invoke(payout);
            PlaySound(sfxWin);
            ShowResult($"¡GANASTE!\n+{payout} fichas", Color.yellow);
            Debug.Log($"[Slot] GANÓ +{payout} | Saldo: {_credits}");
        }
        else
        {
            OnLose?.Invoke();
            PlaySound(sfxLose);
            ShowResult("Inténtalo\nde nuevo", Color.white);
            Debug.Log($"[Slot] Perdió | Saldo: {_credits}");
        }

        yield return new WaitForSeconds(resultDisplayTime);
        HideResultText();

        yield return new WaitForSeconds(0.3f);
        _state = State.Idle;
    }

    // =============================================================
    //  LÓGICA DE JUEGO
    // =============================================================

    private int[] GenerateResult()
    {
        int symbolCount = (reels != null && reels.Length > 0 && reels[0].symbolMaterials != null)
                          ? reels[0].symbolMaterials.Length : 4;
        int reelCount = reels != null ? reels.Length : 4;
        var res = new int[reelCount];

        _spinCount++;

        // ── MODO TEST: cada 5 tiradas forzar victoria ─────────────
        // Quitar este bloque cuando el juego esté listo para producción
        if (_spinCount % 5 == 0)
        {
            int forcedSymbol = Random.Range(0, symbolCount);
            for (int i = 0; i < reelCount; i++)
                res[i] = forcedSymbol;
            Debug.Log($"[Slot] 🎰 VICTORIA FORZADA (tirada #{_spinCount}) — símbolo {forcedSymbol}");
            return res;
        }
        // ──────────────────────────────────────────────────────────

        for (int i = 0; i < reelCount; i++)
            res[i] = Random.Range(0, symbolCount);

        return res;
    }

    /// <summary>
    /// Tabla de pagos para 4 rodillos:
    ///   4 iguales → apuesta × multiplicador
    ///   3 iguales → apuesta × (multiplicador / 2)
    ///   2 iguales → apuesta × (multiplicador / 4)
    ///   sin combinación → 0
    /// </summary>
    private int CalculatePayout(int[] symbols)
    {
        if (symbols == null || symbols.Length != 4)
            return 0;

        Dictionary<int, int> counts = new Dictionary<int, int>();

        foreach (int symbol in symbols)
        {
            if (!counts.ContainsKey(symbol))
                counts[symbol] = 0;

            counts[symbol]++;
        }

        foreach (var pair in counts)
        {
            int symbol = pair.Key;
            int count = pair.Value;

            switch (symbol)
            {
                case 0: // BAR
                    if (count == 4) return 100;
                    if (count == 3) return 50;
                    if (count == 2) return 25;
                    break;

                case 1: // Cherry
                    if (count == 4) return 30;
                    if (count == 3) return 15;
                    if (count == 2) return 7;
                    break;

                case 2: // Lemon
                    if (count == 4) return 20;
                    if (count == 3) return 10;
                    if (count == 2) return 5;
                    break;

                case 3: // Seven
                    if (count == 4) return 50;
                    if (count == 3) return 25;
                    if (count == 2) return 12;
                    break;
            }
        }

        return 0;
    }

    private int GetMult(int id) =>
        (payMultipliers != null && id < payMultipliers.Length) ? payMultipliers[id] : 0;

    // =============================================================
    //  UI
    // =============================================================

    private void UpdateCreditsUI()
    {
        if (creditsText != null)
            creditsText.text = $"Fichas: {_credits}";
    }

    private void ShowResult(string msg, Color color)
    {
        if (resultText == null) return;
        resultText.text = msg;
        resultText.color = color;
        resultText.gameObject.SetActive(true);
    }

    private void HideResultText()
    {
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    // =============================================================
    //  API PÚBLICA
    // =============================================================

    public void AddCredits(int amount) { _credits += amount; UpdateCreditsUI(); }
    public int GetCredits() => _credits;
    public bool IsBusy() => _state != State.Idle;

    // =============================================================
    //  AUDIO
    // =============================================================

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // =============================================================
    //  VALIDACIÓN EN AWAKE
    // =============================================================

    private void CheckSetup()
    {
        if (reels == null || reels.Length == 0)
            Debug.LogWarning("[Slot] ⚠ Asigna los ReelStrip en el array 'reels'.");
        else
            foreach (var r in reels)
                if (r == null) Debug.LogWarning("[Slot] ⚠ Hay un hueco vacío en el array 'reels'.");

        if (creditsText == null) Debug.LogWarning("[Slot] ⚠ Asigna creditsText (TextMeshPro del saldo).");
        if (resultText == null) Debug.LogWarning("[Slot] ⚠ Asigna resultText (TextMeshPro del resultado).");
    }
}
