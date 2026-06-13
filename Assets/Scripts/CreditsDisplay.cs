using UnityEngine;
using TMPro;

/// ================================================================
///  CreditsDisplay.cs  |  Casino VR  |  CS2H01 UTEC 2026
///  
///  Muestra el saldo de fichas del jugador en un TextMeshPro.
///  Conectar a OnCreditsChanged del SlotMachineController.
/// ================================================================

public class CreditsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro creditsText;
    [SerializeField] private string prefix = "Fichas: ";

    // Llamar desde OnCreditsChanged del SlotMachineController
    public void UpdateDisplay(int credits)
    {
        if (creditsText != null)
            creditsText.text = prefix + credits.ToString();
    }
}
