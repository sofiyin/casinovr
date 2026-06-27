using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// ================================================================
///  EndScreenUI.cs  |  Casino VR  |  CS2H01 UTEC 2026
///
///  Pantalla de resumen de fin de sesión (escena "3 Summary Scene").
///  Lee las estadísticas del SessionManager (que persiste entre
///  escenas con DontDestroyOnLoad) y las muestra en un único bloque
///  de texto, con botones para volver a jugar o al menú principal.
/// ================================================================

public class EndScreenUI : MonoBehaviour
{
    [Header("── UI ──")]
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    [Header("── Índices de escena ──")]
    [SerializeField] private int gameSceneIndex = 1;
    [SerializeField] private int mainMenuSceneIndex = 0;

    private void Start()
    {
        if (playAgainButton != null) playAgainButton.onClick.AddListener(PlayAgain);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(MainMenu);

        BuildSummary();
    }

    private void BuildSummary()
    {
        if (summaryText == null) return;

        var s = SessionManager.Instance;
        if (s == null)
        {
            summaryText.text = "No hay datos de sesión.\n(Juega una partida primero.)";
            return;
        }

        float winRate = s.TotalSpins > 0 ? (s.TotalWins * 100f / s.TotalSpins) : 0f;
        int min = (int)(s.SessionDuration / 60f);
        int sec = (int)(s.SessionDuration % 60f);
        string netSign  = s.NetResult >= 0 ? "+" : "";
        string netColor = s.NetResult > 0 ? "#33FF66" : (s.NetResult < 0 ? "#FF4D4D" : "#FFFFFF");

        summaryText.text =
            $"Fichas iniciales:   {s.startingCredits}\n" +
            $"Fichas finales:     {s.currentCredits}\n" +
            $"Resultado neto:     <color={netColor}>{netSign}{s.NetResult}</color>\n" +
            $"\n" +
            $"Giros totales:      {s.TotalSpins}\n" +
            $"Victorias:          {s.TotalWins}  ({winRate:F1}%)\n" +
            $"Derrotas:           {s.TotalLosses}\n" +
            $"Jackpots:           {s.Jackpots}\n" +
            $"\n" +
            $"Mayor ganancia:     {s.BiggestWin}\n" +
            $"Mejor racha:        {s.BestWinStreak}\n" +
            $"Peor racha:         {s.WorstLoseStreak}\n" +
            $"Tiempo jugado:      {min}m {sec}s";
    }

    public void PlayAgain()
    {
        if (SceneTransitionManager.singleton != null)
            SceneTransitionManager.singleton.GoToSceneAsync(gameSceneIndex);
    }

    public void MainMenu()
    {
        if (SceneTransitionManager.singleton != null)
            SceneTransitionManager.singleton.GoToSceneAsync(mainMenuSceneIndex);
    }
}
