using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 마커 삭제 영역 UI 컨트롤러
/// - 드래그 중 삭제 패널 표시 / 숨김
/// - 포인터가 삭제 영역 위에 있는지 판정
/// - New Input System 기준
/// </summary>
public class MarkerDeletePanel : MonoBehaviour
{
    [Header("UI Root")]
    [SerializeField] private GameObject root;

    private RectTransform rect;

    private void Awake()
    {
        rect = root.transform as RectTransform;
        root.SetActive(false);
    }

    // =========================
    // Visibility Control
    // =========================
    public void Show()
    {
        if (!root.activeSelf)
            root.SetActive(true);
    }

    public void Hide()
    {
        if (root.activeSelf)
            root.SetActive(false);
    }

    // =========================
    // Pointer Check (New Input)
    // =========================
    public bool IsPointerOver()
    {
        if (!root.activeSelf)
            return false;

        if (Pointer.current == null)
            return false;

        Vector2 screenPos = Pointer.current.position.ReadValue();

        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            screenPos,
            null   // Screen Space Overlay 기준
        );
    }
}
