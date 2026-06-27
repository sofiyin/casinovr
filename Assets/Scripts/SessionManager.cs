using UnityEngine;

/// ================================================================
///  SessionManager.cs  |  Casino VR  |  CS2H01 UTEC 2026
///
///  Singleton que registra todas las estadísticas de la sesión:
///  dinero inicial/final, giros, rachas, ganancia máxima, etc.
///  Llamar desde SlotMachineController en cada spin.
/// ================================================================

public class SessionManager : MonoBehaviour
{
    // ── SINGLETON ─────────────────────────────────────────────────
    public static SessionManager Instance { get; private set; }

    // ── DATOS DE SESIÓN ───────────────────────────────────────────
    [HideInInspector] public int startingCredits;
    [HideInInspector] public int currentCredits;

    // Contadores
    private int _totalSpins       = 0;
    private int _totalWins        = 0;
    private int _totalLosses      = 0;
    private int _jackpots         = 0;
    private int _totalEarned      = 0;   // suma de todos los premios
    private int _totalSpent       = 0;   // suma de todas las apuestas
    private int _biggestWin       = 0;
    private int _biggestLoss      = 0;   // mayor payout negativo (pérdida de apuesta)

    // Rachas
    private int _currentWinStreak  = 0;
    private int _currentLoseStreak = 0;
    private int _bestWinStreak     = 0;
    private int _worstLoseStreak   = 0;

    // Tiempo
    private float _sessionStartTime;

    // ── PROPIEDADES PÚBLICAS (solo lectura) ───────────────────────
    public int TotalSpins        => _totalSpins;
    public int TotalWins         => _totalWins;
    public int TotalLosses       => _totalLosses;
    public int Jackpots          => _jackpots;
    public int TotalEarned       => _totalEarned;
    public int TotalSpent        => _totalSpent;
    public int BiggestWin        => _biggestWin;
    public int BiggestLoss       => _biggestLoss;
    public int BestWinStreak     => _bestWinStreak;
    public int WorstLoseStreak   => _worstLoseStreak;
    public int NetResult         => currentCredits - startingCredits;
    public float SessionDuration => Time.time - _sessionStartTime;

    // =============================================================
    //  UNITY
    // =============================================================

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =============================================================
    //  INICIAR SESIÓN
    //  Llamar una vez al comenzar (desde SlotMachineController.Awake)
    // =============================================================

    public void StartSession(int initialCredits)
    {
        startingCredits   = initialCredits;
        currentCredits    = initialCredits;
        _sessionStartTime = Time.time;

        _totalSpins = _totalWins = _totalLosses = _jackpots = 0;
        _totalEarned = _totalSpent = _biggestWin = _biggestLoss = 0;
        _currentWinStreak = _currentLoseStreak = 0;
        _bestWinStreak = _worstLoseStreak = 0;

        Debug.Log($"[Session] Sesión iniciada con {initialCredits} fichas.");
    }

    // =============================================================
    //  REGISTRAR GIRO
    //  payout = 0 si perdió, > 0 si ganó, isJackpot si fue jackpot
    // =============================================================

    public void RegisterSpin(int betAmount, int payout, bool isJackpot = false)
    {
        _totalSpins++;
        _totalSpent += betAmount;
        currentCredits -= betAmount;

        if (payout > 0)
        {
            // ── VICTORIA ─────────────────────────────────────────
            _totalWins++;
            _totalEarned  += payout;
            currentCredits += payout;

            if (payout > _biggestWin)   _biggestWin = payout;
            if (isJackpot)              _jackpots++;

            _currentWinStreak++;
            _currentLoseStreak = 0;
            if (_currentWinStreak > _bestWinStreak)
                _bestWinStreak = _currentWinStreak;
        }
        else
        {
            // ── DERROTA ───────────────────────────────────────────
            _totalLosses++;
            if (betAmount > _biggestLoss) _biggestLoss = betAmount;

            _currentLoseStreak++;
            _currentWinStreak = 0;
            if (_currentLoseStreak > _worstLoseStreak)
                _worstLoseStreak = _currentLoseStreak;
        }

        Debug.Log($"[Session] Spin #{_totalSpins} | Apuesta:{betAmount} Pago:{payout} | Saldo:{currentCredits}");
    }

    // =============================================================
    //  ACTUALIZAR CRÉDITOS (para sincronizar con SlotMachine)
    // =============================================================

    public void SyncCredits(int credits)
    {
        currentCredits = credits;
    }

    // =============================================================
    //  RESUMEN COMO STRING (útil para debug)
    // =============================================================

    public string GetSummaryDebug()
    {
        float minutes = SessionDuration / 60f;
        float winRate = _totalSpins > 0 ? (_totalWins * 100f / _totalSpins) : 0f;

        return $"=== RESUMEN DE SESIÓN ===\n" +
               $"Fichas iniciales : {startingCredits}\n" +
               $"Fichas finales   : {currentCredits}\n" +
               $"Resultado neto   : {(NetResult >= 0 ? "+" : "")}{NetResult}\n" +
               $"Giros totales    : {_totalSpins}\n" +
               $"Victorias        : {_totalWins} ({winRate:F1}%)\n" +
               $"Derrotas         : {_totalLosses}\n" +
               $"Jackpots         : {_jackpots}\n" +
               $"Mayor ganancia   : {_biggestWin}\n" +
               $"Racha ganadora   : {_bestWinStreak}\n" +
               $"Racha perdedora  : {_worstLoseStreak}\n" +
               $"Tiempo jugado    : {(int)minutes}m {(int)(SessionDuration % 60)}s";
    }
}
