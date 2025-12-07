using UnityEngine;

public class DeleteButtonUI : MonoBehaviour
{
    public RectTransform ui;
    public Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        Hide();
    }

    public void ShowAt(Vector3 worldPos)
    {
        ui.gameObject.SetActive(true);

        Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
        ui.position = screenPos;
    }

    public void Hide()
    {
        ui.gameObject.SetActive(false);
    }
}
