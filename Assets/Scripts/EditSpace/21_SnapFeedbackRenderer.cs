using UnityEngine;

public class SnapFeedbackRenderer : MonoBehaviour
{
    public LineRenderer lrA; // 첫 번째 선
    public LineRenderer lrB; // 두 번째 선 (corner snap)

    private void Awake()
    {
        SetupLine(lrA);
        SetupLine(lrB);
    }

    private void SetupLine(LineRenderer lr)
    {
        lr.positionCount = 2;
        lr.enabled = false;
        lr.useWorldSpace = true;
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(1f, 0.6f, 0.1f, 1f);
        lr.endColor = new Color(1f, 0.6f, 0.1f, 1f);
    }

    public void ShowEdge(Vector3 a, Vector3 b)
    {
        lrA.enabled = true;
        lrA.SetPosition(0, a + Vector3.up * 0.02f);
        lrA.SetPosition(1, b + Vector3.up * 0.02f);

        lrB.enabled = false;
    }

    public void ShowCorner(Vector3 a1, Vector3 b1, Vector3 a2, Vector3 b2)
    {
        lrA.enabled = true;
        lrA.SetPosition(0, a1 + Vector3.up * 0.02f);
        lrA.SetPosition(1, b1 + Vector3.up * 0.02f);

        lrB.enabled = true;
        lrB.SetPosition(0, a2 + Vector3.up * 0.02f);
        lrB.SetPosition(1, b2 + Vector3.up * 0.02f);
    }

    public void Hide()
    {
        lrA.enabled = false;
        lrB.enabled = false;
    }
}
