using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PPEPickInteraction : BaseInteraction
{
    public List<GameObject> ppeItems = new();
    public ModuleMode moduleMode;

    public override IEnumerator Process()
    {

        foreach (var item in ppeItems)
        {
            var interactable = item.GetComponent<XRBaseInteractable>();
            if (interactable != null)
            {
                interactable.hoverEntered.AddListener((args) =>
                {
                    Debug.Log($"Picked up {item.name}");
                    item.SetActive(false);
                });
            }
            if (moduleMode == ModuleMode.Training) interactable.enabled = false;
            item.SetActive(true);
        }

        if (moduleMode == ModuleMode.Training)
        {
            foreach (var item in ppeItems)
            {
                var interactable = item.GetComponent<XRBaseInteractable>();
                if (interactable != null)
                {
                    interactable.enabled = true;
                }
                if (GuidingArrow.Instance != null) GuidingArrow.Instance.SetTarget(item.transform);
                if (Annotation.Instance != null) Annotation.Instance.Annotate(item.transform, Vector3.up * 0.1f, Color.white, $"Pick up {item.name}");
                item.SetLayerRecursively(LayerMask.NameToLayer("Highlight"));
                yield return new WaitUntil(() => !item.activeSelf);

                if (GuidingArrow.Instance != null) GuidingArrow.Instance.ClearTarget();
                if (Annotation.Instance != null) Annotation.Instance.Annotate();
                
                yield return new WaitForSeconds(1f);
            }
        }
        else
        {
            foreach (var item in ppeItems)
            {
                var interactable = item.GetComponent<XRBaseInteractable>();
                if (interactable != null)
                {
                    interactable.enabled = true;
                }
            }
            yield return new WaitUntil(() => ppeItems.TrueForAll(item => !item.activeSelf));
        }
        "All items picked up".Print();
        FinishInteraction();
        GameManager.Instance.PlayNextInteraction();
        GameManager.Instance.NextButton.gameObject.SetActive(false);
    }
}
