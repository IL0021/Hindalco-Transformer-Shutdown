using UnityEngine;

public class Highlighter : MonoBehaviour
{
    public Material highlightMaterial;
    [SerializeField, HideInInspector]
    private GameObject highlightsContainer;

    public RuntimeAnimatorController animatorController;

    public GameObject handObject;

    void Start()
    {
        highlightMaterial = Resources.Load<Material>("Materials/Highlight");
        // CreateHighlights();
        handObject = Resources.Load<GameObject>("Prefabs/HandObject");
    }

    [ContextMenu("Create Highlights")]
    public void CreateHighlights()
    {
        ClearHighlights();

        if (highlightMaterial == null)
        {
            Debug.LogError("Highlight Material is not assigned.");
            return;
        }

        highlightsContainer = new GameObject("HIGHLIGHTS");
        highlightsContainer.transform.SetParent(this.transform, false);

        if(handObject != null)
        {
            GameObject hand = Instantiate(handObject, highlightsContainer.transform);
        }


        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            if (r.transform.IsChildOf(highlightsContainer.transform))
                continue;

            Mesh mesh = null;
            if (r is MeshRenderer)
            {
                MeshFilter mf = r.GetComponent<MeshFilter>();
                if (mf != null) mesh = mf.sharedMesh;
            }
            else if (r is SkinnedMeshRenderer smr)
            {
                mesh = smr.sharedMesh;
            }

            if (mesh == null) continue;

            GameObject duplicate = new GameObject(r.name + "_Highlight");
            duplicate.transform.position = r.transform.position;
            duplicate.transform.rotation = r.transform.rotation;
            duplicate.transform.localScale = r.transform.lossyScale * 1.1f;

            duplicate.transform.SetParent(highlightsContainer.transform, true);

            MeshFilter destMf = duplicate.AddComponent<MeshFilter>();
            destMf.sharedMesh = mesh;

            MeshRenderer destMr = duplicate.AddComponent<MeshRenderer>();
            destMr.sharedMaterial = highlightMaterial;
        }

        if (animatorController != null)
        {
            var animator = highlightsContainer.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
        }
    }

    [ContextMenu("Clear Highlights")]
    public void ClearHighlights()
    {
        if (highlightsContainer != null) DestroyImmediate(highlightsContainer);

        Transform t = transform.Find("HIGHLIGHTS");
        if (t != null) DestroyImmediate(t.gameObject);
    }
}
