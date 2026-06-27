using System.Collections;
using UnityEngine;

/// ================================================================
///  RecenterMenuToPlayer.cs  |  Casino VR  |  CS2H01 UTEC 2026
///
///  Coloca este script en el contenedor del menú de inicio.
///  Al arrancar, gira todo el grupo del menú alrededor del jugador
///  según hacia dónde esté mirando (yaw), de modo que el menú
///  quede SIEMPRE justo enfrente — sin importar cómo esté orientado
///  el jugador al ponerse el casco. Así las manos apuntan a los
///  botones sin tener que girarse.
///
///  Solo recentra una vez, poco después de iniciar (cuando el
///  tracking del visor ya es válido).
/// ================================================================

public class RecenterMenuToPlayer : MonoBehaviour
{
    [Tooltip("Cámara del jugador (Main Camera del XR Origin). Si se deja vacío usa Camera.main.")]
    [SerializeField] private Transform playerCamera;

    [Tooltip("Segundos a esperar tras iniciar para que el tracking del visor se estabilice.")]
    [SerializeField] private float settleDelay = 0.3f;

    private void OnEnable()
    {
        StartCoroutine(RecenterDelayed());
    }

    private IEnumerator RecenterDelayed()
    {
        yield return new WaitForSeconds(settleDelay);
        Recenter();
    }

    /// <summary>Gira el menú para que quede frente al jugador (solo yaw).</summary>
    public void Recenter()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
        if (playerCamera == null)
        {
            Debug.LogWarning("[RecenterMenu] No hay cámara de jugador asignada.");
            return;
        }

        float yaw = playerCamera.eulerAngles.y;
        Vector3 pivot = new Vector3(playerCamera.position.x, 0f, playerCamera.position.z);
        transform.RotateAround(pivot, Vector3.up, yaw);

        Debug.Log($"[RecenterMenu] Menú recentrado al jugador (yaw {yaw:F1}°).");
    }
}
