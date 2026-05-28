using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.XR.CoreUtils;

public static class FixXROriginTracking
{
    public static void Execute()
    {
        var origin = Object.FindFirstObjectByType<XROrigin>();
        if (origin == null)
        {
            Debug.LogError("No XROrigin found in scene.");
            return;
        }

        Undo.RecordObject(origin, "Fix XR Origin Tracking");
        origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
        origin.CameraYOffset = 0f;

        EditorUtility.SetDirty(origin);
        EditorSceneManager.MarkSceneDirty(origin.gameObject.scene);

        Debug.Log($"[FixXROriginTracking] Set RequestedTrackingOriginMode=Floor, CameraYOffset=0 on {origin.name}");
    }
}
