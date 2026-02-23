using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TeleportInteraction : BaseInteraction
{
    public Transform targetLocation;
    public float delayTime = 1f;
    public float fadeDuration = 1f;
    [SerializeField] XROrigin origin;
    [SerializeField] Image fade;
    public TeleportType teleportType;
    void Awake()
    {
        origin = FindObjectOfType<XROrigin>();
        fade = origin.GetComponentInChildren<Image>();
    }

    public override IEnumerator Process()
    {
        if (teleportType == TeleportType.Object)
        {
            PathDirector.instance.SetTarget(targetLocation);
            GameManager.Instance.teleportationEnabled = true;
            FinishInteraction();
            yield break;
        }
        else
        {
            "Arrows are being cleared".Print();
            PathDirector.instance.ClearTarget();
            GameManager.Instance.teleportationEnabled = false;
        }
        yield return Fade(Color.black);

        yield return new WaitForSeconds(delayTime);
        Debug.Log($"Teleporting to {targetLocation}");
        if (targetLocation != null)
        {
            origin.transform.position = targetLocation.position;
            // origin.transform.rotation = targetLocation.rotation;
            origin.transform.rotation = targetLocation.localRotation;
        }

        yield return Fade(Color.clear);


        FinishInteraction();
    }

    IEnumerator Fade(Color targetColor)
    {
        float timer = 0;
        Color startColor = fade.color;

        while (timer < fadeDuration)
        {
            fade.color = Color.Lerp(startColor, targetColor, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        fade.color = targetColor;
    }

    void OnDrawGizmos()
    {
        if (targetLocation != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(targetLocation.position, 0.2f);
        }
    }
}

public enum TeleportType
{
    Object,
    Location
}
