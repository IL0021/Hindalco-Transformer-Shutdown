using System;
using TMPro;
using UnityEngine;

public class Annotation : MonoBehaviour
{

    public static Annotation Instance;

    [SerializeField] Transform Canvas;
    [SerializeField] TextMeshProUGUI annotationText;
    [SerializeField] Transform targetPosition;
    [SerializeField] Transform lineTargetPosition;

    [SerializeField] float followSpeed;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform lineEndPos;
    [SerializeField] float resolution;
    [SerializeField] private float tangentLength;
    [SerializeField] private CanvasGroup canvasGroup;

    bool enable;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    // void Awake()
    // {
    //     if(targetPosition == null)
    //     {
    //         targetPosition = transform.parent;
    //         lineTargetPosition = transform.parent;
    //     }

    // }

    void LateUpdate()
    {
        if (targetPosition != null)
            Canvas.transform.position = Vector3.Lerp(Canvas.position, targetPosition.position, followSpeed * Time.deltaTime);

        canvasGroup.alpha = snappedLerp(canvasGroup.alpha, enable ? 1 : 0, 5);

        if (lineEndPos && enable)
        {
            lineRenderer.enabled = true;
            if (lineTargetPosition == null) lineTargetPosition = targetPosition;
            Vector3 startPos = lineTargetPosition.position;
            Vector3 startForward = lineTargetPosition.forward;

            Vector3 endPos = lineEndPos.position;
            Vector3 endForward = lineEndPos.forward;

            UpdateBezierLine(startPos, startForward, endPos, endForward);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    float snappedLerp(float from, float to, float speed)
    {
        float result = Mathf.Lerp(from, to, speed * Time.deltaTime);
        if (Mathf.Abs(result - to) < 0.01f)
        {
            result = to;
        }
        return result;
    }
    public void UpdateBezierLine(Vector3 start, Vector3 startTangentDir, Vector3 end, Vector3 endTangentDir)
    {
        if (lineRenderer == null || resolution < 2) return;

        int pointCount = Mathf.CeilToInt(resolution);
        lineRenderer.positionCount = pointCount;

        float tangentScale = tangentLength; // exposed in Inspector

        Vector3 p0 = start;
        Vector3 p1 = start + startTangentDir.normalized * tangentScale;
        Vector3 p2 = end - endTangentDir.normalized * tangentScale;
        Vector3 p3 = end;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);
            Vector3 point = CalculateCubicBezierPoint(t, p0, p1, p2, p3);
            lineRenderer.SetPosition(i, point);
        }
    }

    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        return uuu * p0 +
               3 * uu * t * p1 +
               3 * u * tt * p2 +
               ttt * p3;
    }
    public void Annotate(Transform target, Vector3 offset, Color color, string text)
    {
        annotationText.text = text;
        targetPosition.position = target.position + offset;
        lineTargetPosition = target;
        lineRenderer.startColor = color;
        enable = true;
    }
    public void Annotate(Transform target, Vector3 offset, Transform lineTarget, Color color, string text)
    {
        annotationText.text = text;
        targetPosition.position = target.position + offset;
        lineTargetPosition = lineTarget;
        lineRenderer.startColor = color;
        enable = true;
    }
    public void Annotate()
    {
        enable = false;
    }
    public void Hide()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, 10 * Time.deltaTime);
    }
}