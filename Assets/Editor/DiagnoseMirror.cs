using UnityEditor;
using UnityEngine;
using Unity.XR.CoreUtils;

public static class DiagnoseMirror
{
    public static void Execute()
    {
        var origin = Object.FindFirstObjectByType<XROrigin>();
        if (origin == null) { Debug.LogError("No XROrigin"); return; }

        var cam = origin.Camera.transform;

        // Walk up the full parent chain reporting each localScale
        var t = cam;
        while (t != null)
        {
            Debug.Log($"[Mirror] {t.name}: localScale={t.localScale}, localRot={t.localEulerAngles}");
            t = t.parent;
        }

        // Definitive test: determinant of the camera's world matrix.
        // Negative => the transform includes a reflection (mirror).
        var m = cam.localToWorldMatrix;
        float det = m.determinant;
        Debug.Log($"[Mirror] Camera world matrix determinant = {det}  ({(det < 0 ? "MIRRORED (negative scale somewhere)" : "OK, no mirror in Unity")})");

        Debug.Log($"[Mirror] Camera lossyScale = {cam.lossyScale}");
    }
}
