using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public static class DiagnoseXR
{
    public static void Execute()
    {
        var origin = Object.FindFirstObjectByType<XROrigin>();
        if (origin == null) { Debug.LogError("No XROrigin"); return; }

        Debug.Log($"[Diag] XROrigin world pos: {origin.transform.position}, rot: {origin.transform.eulerAngles}, scale: {origin.transform.lossyScale}");
        Debug.Log($"[Diag] CameraOffset local: pos={origin.CameraFloorOffsetObject.transform.localPosition}, scale={origin.CameraFloorOffsetObject.transform.localScale}");
        Debug.Log($"[Diag] Camera local: pos={origin.Camera.transform.localPosition}, scale={origin.Camera.transform.localScale}");
        Debug.Log($"[Diag] TrackingMode={origin.RequestedTrackingOriginMode}, YOffset={origin.CameraYOffset}");

        var snap = Object.FindFirstObjectByType<ActionBasedSnapTurnProvider>();
        if (snap != null)
        {
            Debug.Log($"[Diag] SnapTurn: amount={snap.turnAmount}, debounce={snap.debounceTime}");
        }

        var cont = Object.FindFirstObjectByType<ActionBasedContinuousTurnProvider>();
        if (cont != null)
        {
            Debug.Log($"[Diag] ContinuousTurn: speed={cont.turnSpeed}, enabled={cont.enabled}");
        }
    }

    public static void FixCameraOffset()
    {
        var origin = Object.FindFirstObjectByType<XROrigin>();
        if (origin == null) { Debug.LogError("No XROrigin"); return; }

        var offsetTr = origin.CameraFloorOffsetObject.transform;
        Undo.RecordObject(offsetTr, "Fix CameraOffset Y");
        var lp = offsetTr.localPosition;
        lp.y = 0f;
        offsetTr.localPosition = lp;

        EditorUtility.SetDirty(offsetTr);
        EditorSceneManager.MarkSceneDirty(origin.gameObject.scene);
        Debug.Log($"[FixCameraOffset] CameraOffset local Y set to 0. Floor tracking will now use only the headset's actual height.");
    }

    public static void RevertHeight()
    {
        var origin = Object.FindFirstObjectByType<XROrigin>();
        if (origin == null) { Debug.LogError("No XROrigin"); return; }
        Undo.RecordObject(origin, "Revert Height");
        origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.NotSpecified;
        origin.CameraYOffset = 1.36144f;
        EditorUtility.SetDirty(origin);
        EditorSceneManager.MarkSceneDirty(origin.gameObject.scene);
        Debug.Log("[RevertHeight] Reverted to NotSpecified / 1.36144");
    }
}
