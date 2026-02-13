using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanvasTransformParent : MonoBehaviour
{
    [SerializeField] Color childColor;
    void OnDrawGizmos()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
                Gizmos.color = childColor;
            else
                Gizmos.color = Color.gray;
            var matrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(child.position, child.rotation, child.localScale);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = matrix;
#if UNITY_EDITOR            
            Handles.color = childColor;
            Handles.Label(child.position, child.name, new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = childColor },
                fontSize = 16,
                fontStyle = FontStyle.Bold
            });
            // Handles.PositionHandle(child.localPosition, child.localRotation);
#endif
        }
    }

    [ContextMenu("Rename")]
    public void Rename()
    {
        int i = 1;
        foreach (Transform child in transform)
        {
            child.name = $"Step_{i}";
#if UNITY_EDITOR
            EditorUtility.SetDirty(child);
#endif
            i++;
        }
    }
}
