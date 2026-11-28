using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SecurityCameraVision2 : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 8f;
    [SerializeField, Range(1f, 179f)] private float visionAngle = 60f;
    [SerializeField, Range(3, 100)] private int segments = 48;

    [Header("Detection")]
    [SerializeField] private Transform player;

    public event Action OnPlayerDetected;

    private bool playerDetected;

    private Mesh visionMesh;
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateVisionMesh();
    }

    private void Update()
    {
        CheckPlayer();
    }

    private void CheckPlayer()
    {
        if (player == null)
            return;

        Vector3 directionToPlayer = player.position - transform.position;

        float distance = directionToPlayer.magnitude;

        if (distance > visionRange)
        {
            playerDetected = false;
            return;
        }

        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle <= visionAngle / 2f)
        {
            if (!playerDetected)
            {
                playerDetected = true;
                Debug.Log("Player Detectado");
                OnPlayerDetected?.Invoke();
            }
        }
        else
        {
            playerDetected = false;
        }
    }

    private void GenerateVisionMesh()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if (visionMesh != null)
        {
            if (Application.isPlaying)
                Destroy(visionMesh);
            else
                DestroyImmediate(visionMesh);
        }

        visionMesh = new Mesh();
        visionMesh.name = "Security Camera Vision";

        // Create vertices
        Vector3[] vertices = new Vector3[segments + 2];

        // Cone tip
        vertices[0] = Vector3.zero;

        float radius = visionRange *
                       Mathf.Tan(visionAngle * 0.5f * Mathf.Deg2Rad);

        // Generate circular base vertices
        for (int i = 0; i < segments; i++)
        {
            float angle = i * (360f / segments) * Mathf.Deg2Rad;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            vertices[i + 1] = new Vector3(x, y, visionRange);
        }

        // Base center
        int centerIndex = segments + 1;
        vertices[centerIndex] = new Vector3(0f, 0f, visionRange);

        // Create triangles
        int[] triangles = new int[segments * 6];

        for (int i = 0; i < segments; i++)
        {
            int current = i + 1;
            int next = ((i + 1) % segments) + 1;

            int triangleIndex = i * 6;

            // Cone lateral surface
            triangles[triangleIndex] = 0;
            triangles[triangleIndex + 1] = next;
            triangles[triangleIndex + 2] = current;

            // Cone base
            triangles[triangleIndex + 3] = centerIndex;
            triangles[triangleIndex + 4] = current;
            triangles[triangleIndex + 5] = next;
        }

        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;

        visionMesh.RecalculateNormals();
        visionMesh.RecalculateBounds();

        meshFilter.sharedMesh = visionMesh;
    }

    private void OnValidate()
    {
        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            GenerateVisionMesh();
        }
    }

    private void OnDestroy()
    {
        if (visionMesh != null)
        {
            Destroy(visionMesh);
        }
    }
}