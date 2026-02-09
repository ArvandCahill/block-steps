using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rigidbody))]
public class RigidbodyEditor : Editor
{
    void OnSceneGUI()
    {
        Rigidbody rb = (Rigidbody)target;
        if (rb == null) return;

        Handles.color = Color.red;

        Vector3 worldCenterOfMass =
            rb.transform.TransformPoint(rb.centerOfMass);

        Handles.SphereHandleCap(
            0,
            worldCenterOfMass,
            Quaternion.identity,
            0.2f,
            EventType.Repaint
        );
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
