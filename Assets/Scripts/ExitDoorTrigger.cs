using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// ================================================================
///  ExitDoorTrigger.cs  |  Casino VR  |  CS2H01 UTEC 2026
///
///  Coloca este script en la puerta de salida del casino VR.
///  Cuando el jugador se acerca (Trigger de colisión) aparece
///  un Canvas de confirmación "¿Deseas salir?" en el mundo.
///  Si confirma → muestra SessionSummaryUI.
///
///  SETUP en Unity:
///  1. Crea un GameObject "ExitDoor" con un Collider (Is Trigger = ON)
///  2. Adjunta este script
///  3. Crea un Canvas World Space "ConfirmCanvas" hijo de la puerta
///     └─ ConfirmCanvas
///         ├─ PanelFondo
///         ├─ QuestionText  ("¿Deseas salir del casino?")
///         ├─ BtnSi         (XRSimpleInteractable o Button)
///         └─ BtnNo         (XRSimpleInteractable o Button)
///  4. Asigna las referencias en el Inspector
/// ================================================================

public class ExitDoorTrigger : MonoBehaviour
{
    // ── REFERENCIAS ───────────────────────────────────────────────
    [Header("── UI de Confirmación ──")]
    [Tooltip("Canvas World Space que aparece al acercarse a la puerta.")]
    [SerializeField] private GameObject confirmCanvas;

    [Tooltip("Texto opcional que muestra cuántas fichas tiene el jugador al acercarse.")]
    [SerializeField] private TextMeshProUGUI creditsPreviewText;

    [Header("── Pantalla de Resumen ──")]
    [Tooltip("El script SessionSummaryUI de tu escena.")]
    [SerializeField] private SessionSummaryUI summaryUI;

    [Header("── Slot Machine ──")]
    [Tooltip("Referencia al SlotMachineController para saber si está girando.")]
    [SerializeField] private SlotMachineController slotMachine;

    [Header("── Audio ──")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxDoorOpen;
    [SerializeField] private AudioClip sfxConfirm;

    [Header("── Configuración ──")]
    [Tooltip("Tag del jugador XR (normalmente 'Player' o 'MainCamera').")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Animador de la puerta (opcional).")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string doorOpenParam = "Open";

    // ── ESTADO ────────────────────────────────────────────────────
    private bool _playerNearby  = false;
    private bool _waitingConfirm = false;

    // =============================================================
    //  UNITY
    // =============================================================

    private void Awake()
    {
        if (confirmCanvas != null)
            confirmCanvas.SetActive(false);
    }

    // =============================================================
    //  TRIGGER DE COLISIÓN
    //  El jugador entra al área de la puerta
    // =============================================================

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        _playerNearby = true;

        // No interrumpir si la máquina está girando
        if (slotMachine != null && slotMachine.IsBusy())
        {
            Debug.Log("[Door] La máquina está en uso, espera que termine.");
            return;
        }

        ShowConfirmPanel();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        _playerNearby = false;

        // Si el jugador se alejó sin confirmar, cerrar panel
        if (!_waitingConfirm)
            HideConfirmPanel();
    }

    // =============================================================
    //  PANEL DE CONFIRMACIÓN
    // =============================================================

    private void ShowConfirmPanel()
    {
        if (confirmCanvas == null) return;

        // Mostrar fichas actuales en el preview
        if (creditsPreviewText != null && slotMachine != null)
            creditsPreviewText.text = $"Tienes {slotMachine.GetCredits()} fichas.\n¿Deseas salir del casino?";

        confirmCanvas.SetActive(true);

        // Animar la puerta
        if (doorAnimator != null)
            doorAnimator.SetBool(doorOpenParam, true);

        if (audioSource != null && sfxDoorOpen != null)
            audioSource.PlayOneShot(sfxDoorOpen);

        Debug.Log("[Door] Jugador cerca de la salida. Mostrando confirmación.");
    }

    private void HideConfirmPanel()
    {
        if (confirmCanvas != null)
            confirmCanvas.SetActive(false);

        if (doorAnimator != null)
            doorAnimator.SetBool(doorOpenParam, false);
    }

    // =============================================================
    //  BOTONES DEL PANEL DE CONFIRMACIÓN
    //  Conectar estos métodos a los botones desde el Inspector
    //  (o desde XRSimpleInteractable.SelectEntered)
    // =============================================================

    /// <summary>
    /// El jugador confirmó que quiere salir → mostrar resumen.
    /// Conectar al botón "Sí" del confirmCanvas.
    /// </summary>
    public void OnConfirmExit()
    {
        if (audioSource != null && sfxConfirm != null)
            audioSource.PlayOneShot(sfxConfirm);

        HideConfirmPanel();

        if (summaryUI != null)
            summaryUI.ShowSummary();
        else
            Debug.LogWarning("[Door] No hay SessionSummaryUI asignado.");

        Debug.Log("[Door] ✅ Jugador confirmó salida. Mostrando resumen de sesión.");
    }

    /// <summary>
    /// El jugador decidió quedarse.
    /// Conectar al botón "No" del confirmCanvas.
    /// </summary>
    public void OnCancelExit()
    {
        _waitingConfirm = false;
        HideConfirmPanel();
        Debug.Log("[Door] 🔙 Jugador decidió quedarse.");
    }
}
