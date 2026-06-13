using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// ================================================================
///  ReelStrip.cs  |  Casino VR  |  CS2H01 UTEC 2026
///  Compatible: Unity 6 + XR Interaction Toolkit + Meta Quest 2
///
///  Simula un rodillo de slot con un strip vertical de quads que se
///  desplazan hacia abajo. Los que salen por abajo se reciclan al
///  tope con un material nuevo. Sin rotación de quad: scrolleo puro.
///
///  Setup por rodillo:
///   1. Crea un GameObject vacío donde estaba cada quad original.
///   2. Añade este script.
///   3. Asigna los mismos 4 materiales (BAR, Seven, Cherry, Lemon).
///   4. Ajusta symbolHeight y symbolWidth para que coincidan con el
///      tamaño visible del marco del rodillo en tu modelo 3D.
/// ================================================================

public class ReelStrip : MonoBehaviour
{
    // ── MATERIALES ────────────────────────────────────────────────
    [Header("── Materiales de símbolos ──")]
    [Tooltip("Los mismos 4 materiales que tenías en SlotMachineController.\n[0]=BAR  [1]=Seven  [2]=Cherry  [3]=Lemon")]
    public Material[] symbolMaterials;

    // ── DIMENSIONES ───────────────────────────────────────────────
    [Header("── Dimensiones del rodillo ──")]
    [Tooltip("Alto de cada símbolo en unidades Unity. Ajusta hasta que 3 símbolos llenen la ventana visible del modelo.")]
    public float symbolHeight = 0.08f;
    [Tooltip("Ancho de cada símbolo en unidades Unity.")]
    public float symbolWidth  = 0.08f;
    [Range(1, 5)]
    [Tooltip("Filas visibles. Normalmente 3.")]
    public int visibleRows = 3;

    // ── VELOCIDADES ───────────────────────────────────────────────
    [Header("── Velocidades ──")]
    [Tooltip("Velocidad máxima de scroll (unidades/segundo). Empieza con 1.5 y ajusta.")]
    public float maxSpeed     = 1.5f;
    [Tooltip("Aceleración al arrancar el giro.")]
    public float acceleration = 4f;
    [Tooltip("Desaceleración al frenar.")]
    public float deceleration = 3f;

    // ── ESTADO INTERNO ────────────────────────────────────────────
    private List<MeshRenderer> items = new List<MeshRenderer>();
    private float currentSpeed  = 0f;
    private bool  isSpinning    = false;
    private bool  isStopping    = false;
    private int   pendingResult = 0;

    /// <summary>Índice del material visible en la fila central tras el giro.</summary>
    public int  Result     { get; private set; }
    public bool IsSpinning => isSpinning;

    // =============================================================
    //  INICIALIZACIÓN
    // =============================================================

    private void Awake()
    {
        if (symbolMaterials == null || symbolMaterials.Length == 0)
        {
            Debug.LogError("[ReelStrip] Asigna los materiales en 'symbolMaterials'.", this);
            return;
        }
        BuildStrip();
    }

    private void BuildStrip()
    {
        // Total = visibles + 1 buffer arriba + 1 buffer abajo
        int total = visibleRows + 2;

        // y=0 es el centro del rodillo (fila central).
        // Los buffers quedan 1 celda por encima y por debajo del área visible.
        float topY = ((visibleRows - 1) / 2f + 1f) * symbolHeight;

        for (int i = 0; i < total; i++)
        {
            // Quad primitivo de Unity con MeshFilter + MeshRenderer
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "Symbol_" + i;
            go.transform.SetParent(transform);
            go.transform.localScale    = new Vector3(symbolWidth, symbolHeight, 1f);
            go.transform.localPosition = new Vector3(0f, topY - i * symbolHeight, 0f);
            go.transform.localRotation = Quaternion.identity;

            // El collider del quad primitivo no hace falta
            Destroy(go.GetComponent<MeshCollider>());

            var mr     = go.GetComponent<MeshRenderer>();
            mr.material = RandomMaterial();

            // Los buffers (primero y último) son visualmente idénticos
            // al resto; el marco del modelo 3D los ocultará naturalmente.
            // Si necesitas ocultarlos por código, descomenta la línea de abajo:
            // bool isBuffer = (i == 0 || i == total - 1);
            // mr.enabled = !isBuffer;

            items.Add(mr);
        }
    }

    // =============================================================
    //  LOOP PRINCIPAL
    // =============================================================

    private void Update()
    {
        if (currentSpeed <= 0f) return;

        float step = currentSpeed * Time.deltaTime;

        // Mover todo el strip hacia abajo
        foreach (var item in items)
            item.transform.localPosition += Vector3.down * step;

        RecycleSymbols();
    }

    private void RecycleSymbols()
    {
        // Umbral: medio slot extra por debajo del área visible → el recyclado es invisible
        float killY  = -(visibleRows / 2f + 1f) * symbolHeight;
        float totalH =  items.Count * symbolHeight;

        foreach (var item in items)
        {
            if (item.transform.localPosition.y < killY)
            {
                // Reposicionar en la cima manteniendo el spacing exacto
                var pos = item.transform.localPosition;
                item.transform.localPosition = new Vector3(pos.x, pos.y + totalH, pos.z);

                // Material aleatorio mientras gira
                // (al frenar, SnapToResult asigna el resultado final)
                if (!isStopping)
                    item.material = RandomMaterial();
            }
        }
    }

    // =============================================================
    //  API PÚBLICA
    // =============================================================

    /// <summary>Arranca el rodillo. Llamado por SlotMachineController.</summary>
    public void Spin()
    {
        if (isSpinning) return;
        isSpinning = true;
        isStopping = false;
        StartCoroutine(SpinCoroutine());
    }

    /// <summary>
    /// Frena el rodillo y muestra el símbolo indicado en la fila central.
    /// Llamado por SlotMachineController una vez calculado el resultado.
    /// </summary>
    public void Stop(int resultMaterialIndex)
    {
        if (!isSpinning || isStopping) return;
        pendingResult = Mathf.Clamp(resultMaterialIndex, 0, symbolMaterials.Length - 1);
        isStopping    = true;
    }

    // =============================================================
    //  COROUTINE DE GIRO
    // =============================================================

    private IEnumerator SpinCoroutine()
    {
        // 1. Acelerar hasta velocidad máxima
        while (currentSpeed < maxSpeed && !isStopping)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
            yield return null;
        }
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        // 2. Velocidad constante hasta que llegue la orden de parar
        while (!isStopping)
            yield return null;

        // 3. Desacelerar suavemente
        while (currentSpeed > 0.005f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            yield return null;
        }
        currentSpeed = 0f;

        // 4. Alinear el símbolo resultado al centro exacto (y=0)
        SnapToResult(pendingResult);
        Result     = pendingResult;
        isSpinning = false;
    }

    private void SnapToResult(int targetIndex)
    {
        // Buscar el quad más cercano al centro (y = 0)
        MeshRenderer closest = null;
        float minDist = float.MaxValue;

        foreach (var item in items)
        {
            float d = Mathf.Abs(item.transform.localPosition.y);
            if (d < minDist) { minDist = d; closest = item; }
        }

        if (closest == null) return;

        // Asignar material resultado al quad central
        closest.material = symbolMaterials[targetIndex];

        // Desplazar todo el strip para que ese quad quede exactamente en y=0
        // El desfase es siempre < symbolHeight/2, así que no hay salto brusco
        float snapOffset = closest.transform.localPosition.y;
        foreach (var item in items)
        {
            var p = item.transform.localPosition;
            item.transform.localPosition = new Vector3(p.x, p.y - snapOffset, p.z);
        }

        // Materiales aleatorios para el resto de celdas visibles
        foreach (var item in items)
            if (item != closest)
                item.material = RandomMaterial();
    }

    // =============================================================
    //  UTILIDAD
    // =============================================================

    private Material RandomMaterial() =>
        symbolMaterials[Random.Range(0, symbolMaterials.Length)];

#if UNITY_EDITOR
    // Gizmo en el editor: dibuja el área visible del rodillo
    private void OnDrawGizmosSelected()
    {
        float h = visibleRows * symbolHeight;
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(symbolWidth, h, 0.01f));
    }
#endif
}
