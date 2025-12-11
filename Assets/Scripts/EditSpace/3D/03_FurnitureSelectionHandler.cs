using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureSelectionHandler : MonoBehaviour
{
    public static FurnitureSelectionHandler Instance {get; private set;}

    [Header("Raycast")]
    public LayerMask furnitureMask;

    private Camera cam;

    public void SetCamera(Camera newCam) => cam = newCam;

    private void Awake()
    {
        Instance = this;
        Debug.Log(cam);
    }

    private void Update()
    {
        // Debug.Log(InputReader.Instance.Point);
        var mode = EditorModeManager.Instance.CurrentMode;

        if (mode != EditMode.PlaceFurniture && mode != EditMode.EditFurniture)
            return;

        if (GizmoInputBlocker.IsDraggingGizmo)
            return;

        if (InputReader.Instance.ContactStarted)
        {
            if (IsPointerOverUI())
                return;
            HandleClick(InputReader.Instance.Point);
        }
    }

    private void HandleClick(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, furnitureMask))
        {
            FurniturePiece piece = hit.collider.GetComponent<FurniturePiece>();
            if (piece != null)
            {
                FurnitureManager.Instance.Select(piece);

                FurnitureCopyButton.Instance.Show();

                if (FurnitureMoveGizmo.Instance != null)
                    FurnitureGizmoController.Instance.Attach(piece);
                else
                    Debug.Log("gizmo is null");

                return;
            }
        }

        // 빈 공간 클릭 → 선택 해제
        FurnitureManager.Instance.ClearSelection();
        FurnitureCopyButton.Instance.Hide();

        if (FurnitureMoveGizmo.Instance != null)
            FurnitureGizmoController.Instance.Detach();
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject();
#else
        if (UnityEngine.Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(0).fingerId);
        return false;
#endif
    }
}
