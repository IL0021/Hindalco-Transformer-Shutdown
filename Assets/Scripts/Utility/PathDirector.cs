using UnityEngine;
using UnityEngine.AI;

public class PathDirector : MonoBehaviour
{
    public static PathDirector instance;
    private Transform start;
    // private Vector3 end;
    public GameObject DirectionArrow;
    public float prefabSpacing; // Spacing between each prefab

    private NavMeshPath path;
    private GameObject arrowsContainer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        start = this.transform;
        path = new NavMeshPath();
        arrowsContainer = new GameObject("ArrowsContainer");
    }
    // public void SetEndValue(Vector3 EndPosition)
    // {
    //     // end = EndPosition;
    //     CalculatePath();
    //     MovePrefabAlongPath();
    //     print("path created");
    // }

    private void Update()
    {
        if (target == null) return;
        CalculatePath();
        MovePrefabAlongPath();
    }

    //private void SetStartPoint()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        start.position = hit.point;
    //    }
    //} // when starting point is based on the raycasting use this
    public Transform target;
    [ContextMenu("Set Target")]
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        CalculatePath();
        MovePrefabAlongPath();
    }
    private void CalculatePath()
    {
        NavMesh.CalculatePath(start.position, target.position, NavMesh.AllAreas, path);
        // print("path calculator");
    }

    // private void MovePrefabAlongPath()
    // {
    //     ClearArrows();

    //     if (path.corners.Length < 2)
    //     {
    //         return;
    //     }

    //     float distance = 0f;

    //     for (int i = 1; i < path.corners.Length; i++)
    //     {
    //         Vector3 direction = path.corners[i] - path.corners[i - 1];
    //         Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
    //         float segmentDistance = Vector3.Distance(path.corners[i - 1], path.corners[i]);
    //         print(segmentDistance);
    //         while (distance < segmentDistance)
    //         {
    //             Vector3 position = path.corners[i - 1] + (direction.normalized * distance);
    //             var arrowPrefab =   Instantiate(DirectionArrow, position, rotation, arrowsContainer.transform);
    //             print(arrowPrefab.name);
    //             distance += prefabSpacing;
    //             print(distance);
    //         }

    //         distance -= segmentDistance;

    //     }
    // }
    private void MovePrefabAlongPath()
    {
        ClearArrows();

        if (prefabSpacing <= 0.1f)
        {
            Debug.LogWarning("Prefab Spacing is too small!");
            return;
        }

        if (path.corners.Length < 2) return;

        // Reset distance for the start of the whole path
        float distanceCoveredOnSegment = 0f;

        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 startPoint = path.corners[i - 1];
            Vector3 endPoint = path.corners[i];
            float segmentLength = Vector3.Distance(startPoint, endPoint);
            Vector3 direction = (endPoint - startPoint).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Place arrows along this segment
            while (distanceCoveredOnSegment < segmentLength)
            {
                Vector3 spawnPos = startPoint + (direction * distanceCoveredOnSegment);
                Instantiate(DirectionArrow, spawnPos, rotation, arrowsContainer.transform);

                distanceCoveredOnSegment += prefabSpacing;
            }

            // Carry over the remaining distance to the next segment 
            // to keep spacing consistent across corners
            distanceCoveredOnSegment -= segmentLength;
        }
    }

    public void ClearArrows()
    {
        foreach (Transform child in arrowsContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearTarget()
    {
        target = null;
        ClearArrows();
    }
}
