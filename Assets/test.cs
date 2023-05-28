using UnityEngine;

public class test : MonoBehaviour
{
    public Color lineColor = Color.white; // Kolor linii (biały)

    void Start()
    {
        // Tworzenie punktów
        Vector3[] points = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 0f, 1f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, 0f)
        };

        // Dodaj komponent LineRenderer
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Ustaw parametry LineRenderer
        lineRenderer.positionCount = points.Length;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        // Ustaw tryb mieszania materiału na Opaque
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Ustaw pozycje punktów
        lineRenderer.SetPositions(points);
    }
}

