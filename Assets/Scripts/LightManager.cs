using System.Collections;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public MeshRenderer lightMeshRenderer;
    public Material onLight, offLight;
    public Material[] materials;
    IEnumerator Start()
    {
        if (lightMeshRenderer == null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        var breakerInteraction = GetComponent<BreakerInteraction>();
        breakerInteraction.OnTrip += TurnOff;
        breakerInteraction.OnClose += TurnOn;

        materials = lightMeshRenderer.materials;
        foreach (var mat in materials)
        {
            if (mat.name.ToLower().Contains("red"))
            {
                onLight = mat;
            }
            else if (mat.name.ToLower().Contains("green"))
            {
                offLight = mat;
            }
        }
    }
    [ContextMenu("Turn On Light")]
    public void TurnOn()
    {
        onLight.EnableKeyword("_EMISSION");
        offLight.DisableKeyword("_EMISSION");
    }
    [ContextMenu("Turn Off Light")]
    public void TurnOff()
    {
        onLight.DisableKeyword("_EMISSION");
        offLight.EnableKeyword("_EMISSION");
    }

}
