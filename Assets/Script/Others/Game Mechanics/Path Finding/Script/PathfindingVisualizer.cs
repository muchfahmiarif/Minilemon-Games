using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathfindingVisualizer : MonoBehaviour
{
    public Transform target;
    public bool showVirtualCircle = true;
    [SerializeField] private bool isVirtualCircleCreated = false;
    [SerializeField] private float circleRadiusMultiplier = 2.5f;

    public int smoothnessSubdivisions = 1; // Number of subdivisions for smoothing the path
    public float curveSmoothness = 0.5f; // Controls the smoothness of the curves (0 = linear, 1 = fully curved)

    [SerializeField] private float lineRendererOffset = 0.1f; // Offset above mesh surface
    [SerializeField] private float colliderRaycastDistance = 0.2f; // Distance for raycast to detect colliders

    private NavMeshPath navMeshPath;
    private LineRenderer lineRenderer;
    private GameObject virtualCircle;

    void Start()
    {
        navMeshPath = new NavMeshPath();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; // Initialize line renderer with zero points
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = GetVirtualCircleCenter();
            NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, navMeshPath);

            DisplayCurvedPath(navMeshPath);
            UpdateVirtualCircle();
            lineRenderer.enabled = true;
        }
        else 
        {
            lineRenderer.enabled = false;
        }
    }

    private Vector3 GetVirtualCircleCenter()
    {
        if (virtualCircle == null)
            return transform.position;

        return virtualCircle.transform.position;
    }

    private void DisplayCurvedPath(NavMeshPath path)
    {
        if (path.corners.Length < 2) return;

        List<Vector3> smoothedPath = new List<Vector3>();
        smoothedPath.AddRange(path.corners); // Directly add corners for straight lines

        if (path.corners.Length > 2)
        {
            smoothedPath = SmoothPath(path.corners);
            smoothedPath = GenerateCatmullRomSpline(smoothedPath.ToArray(), smoothnessSubdivisions);
        }

        List<Vector3> adjustedPath = new List<Vector3>();
        for (int i = 0; i < smoothedPath.Count; i++)
        {
            Vector3 point = smoothedPath[i];

            // Skip adjusting first and last points (player and target)
            if (i == 0 || i == smoothedPath.Count - 1)
            {
                adjustedPath.Add(point);
                continue;
            }

            RaycastHit hit;
            if (Physics.Raycast(point + Vector3.up * colliderRaycastDistance, Vector3.down, out hit, colliderRaycastDistance + lineRendererOffset))
            {
                // Raise point if it hits a collider (excluding target and player)
                if (hit.collider != target.GetComponent<Collider>() && hit.collider != GetComponent<Collider>())
                {
                    point.y = hit.point.y + lineRendererOffset;
                }
            }

            adjustedPath.Add(point + Vector3.up * lineRendererOffset); // Apply offset
        }

        lineRenderer.positionCount = adjustedPath.Count;
        lineRenderer.SetPositions(adjustedPath.ToArray());
    }

    private List<Vector3> SmoothPath(Vector3[] path)
    {
        List<Vector3> smoothedPath = new List<Vector3>();
        for (int i = 0; i < path.Length; i++)
        {
            smoothedPath.Add(path[i]);
            if (i > 0 && i < path.Length - 1)
            {
                Vector3 previousDir = (path[i] - path[i - 1]).normalized;
                Vector3 nextDir = (path[i + 1] - path[i]).normalized;
                if (Vector3.Dot(previousDir, nextDir) < 0.5f)
                {
                    smoothedPath.Add((path[i] + path[i + 1]) * 0.5f);
                }
            }
        }
        return smoothedPath;
    }

    private List<Vector3> GenerateCatmullRomSpline(Vector3[] points, int smoothness)
    {
        int numPoints = points.Length;
        int splinePointsCount = (numPoints - 1) * smoothness;
        List<Vector3> splinePoints = new List<Vector3>();

        for (int i = 0; i < numPoints - 1; i++)
        {
            Vector3 p0 = i == 0 ? points[0] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i + 2 < numPoints ? points[i + 2] : points[i + 1];

            for (int j = 0; j < smoothness; j++)
            {
                float t = (float)j / smoothness;
                Vector3 point = EvaluateCatmullRom(p0, p1, p2, p3, t);
                splinePoints.Add(point);
            }
        }

        return splinePoints;
    }

    private Vector3 EvaluateCatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 point = 0.5f * (
        (2.0f * p1) +
        (-p0 + p2) * t +
        (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
        (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3
        );

        return point;
    }

    private void UpdateVirtualCircle()
    {
        if (target == null) return;

        if (!isVirtualCircleCreated)
        {
            Bounds targetBounds = target.GetComponent<Renderer>().bounds;
            Vector3 targetCenter = targetBounds.center;
            float circleRadius = Mathf.Max(targetBounds.extents.x, targetBounds.extents.z) * circleRadiusMultiplier;

            virtualCircle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            virtualCircle.name = "VirtualCircle";
            Destroy(virtualCircle.GetComponent<Collider>());
            virtualCircle.transform.position = new Vector3(targetCenter.x, targetBounds.min.y, targetCenter.z);
            virtualCircle.transform.localScale = new Vector3(circleRadius, 0.1f, circleRadius);
            virtualCircle.GetComponent<Renderer>().material.color = Color.white;
            virtualCircle.SetActive(showVirtualCircle);

            isVirtualCircleCreated = true;
        }
        else
        {
            Bounds targetBounds = target.GetComponent<Renderer>().bounds;
            Vector3 targetCenter = targetBounds.center;
            float circleRadius = Mathf.Max(targetBounds.extents.x, targetBounds.extents.z) * circleRadiusMultiplier;

            virtualCircle.transform.position = new Vector3(targetCenter.x, targetBounds.min.y, targetCenter.z);
            virtualCircle.transform.localScale = new Vector3(circleRadius, 0.1f, circleRadius);
            virtualCircle.SetActive(showVirtualCircle);
        }
    }

    private void OnDrawGizmos()
    {
        if (showVirtualCircle && virtualCircle != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(virtualCircle.transform.position, virtualCircle.transform.localScale.x / 2);
        }
    }
}
