using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// ================================================================
///  SpinButton.cs  |  Casino VR  |  CS2H01 UTEC 2026
///  
///  Adjuntar al GameObject "Button" que ya existe bajo SlotMachine.
///  Requiere también un XRSimpleInteractable en el mismo objeto.
/// ================================================================

[RequireComponent(typeof(XRSimpleInteractable))]
public class SpinButton : MonoBehaviour
{
    [Header("── Referencia ──")]
    [Tooltip("Arrastra aquí el GameObject 'SlotMachine' (el que tiene SlotMachineController).")]
    [SerializeField] private SlotMachineController slotMachine;

    [Header("── Animación del botón ──")]
    [Tooltip("Cuánto se hunde el botón al presionar (eje Y local).")]
    [SerializeField] private float pressDepth = 0.01f;
    [SerializeField] private float pressSpeed = 20f;

    // ──────────────────────────────────────────────────
    private XRSimpleInteractable _interactable;
    private Vector3 _restPos;
    private Vector3 _targetPos;

    private void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        _restPos      = transform.localPosition;
        _targetPos    = _restPos;
    }

    private void OnEnable()
    {
        _interactable.selectEntered.AddListener(OnPress);
        _interactable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        _interactable.selectEntered.RemoveListener(OnPress);
        _interactable.selectExited.RemoveListener(OnRelease);
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition, _targetPos, Time.deltaTime * pressSpeed);
    }

    private void OnPress(SelectEnterEventArgs _)
    {
        _targetPos = _restPos + Vector3.down * pressDepth;
        if (slotMachine != null) slotMachine.TrySpin();
        else Debug.LogWarning("[SpinButton] ⚠ Asigna SlotMachineController.");
    }

    private void OnRelease(SelectExitEventArgs _)
    {
        _targetPos = _restPos;
    }
}
