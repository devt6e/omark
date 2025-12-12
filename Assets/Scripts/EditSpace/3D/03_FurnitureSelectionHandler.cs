using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureSelectionHandler : MonoBehaviour
{
    [Header("Raycast")]
    public LayerMask furnitureMask;

    private Camera cam;

    private void Awake()
    {
        cam = GlobalCameraManager.Camera3D;
    }

    private void Update()
    {
        // Debug.Log(InputReader.Instance.Point);
        var mode = EditorModeManager.Instance.CurrentMode;

        if (mode != EditMode.PlaceFurniture && mode != EditMode.EditFurniture)
            return;

        if (GizmoInputBlocker.IsDraggingGizmo)
            return;

        // Debug.Log("POINT = " + InputReader.Instance.Point + 
        //   "   Screen = " + Screen.width + ", " + Screen.height);
        if (InputReader.Instance.ContactStarted)
        {
            if (IsPointerOverUI())
                return;
            Debug.Log("Ray Shooooot");
            HandleClick(InputReader.Instance.Point);
        }
    }

    private void HandleClick(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f)) // LayerMask 제거
        {
            Debug.Log("Hit ANY collider: " + hit.collider.name);
        }
        else
        {
            Debug.Log("Raycast did NOT hit anything at all.");
        }
        // Debug.Log("hit0");
        // if (Physics.Raycast(ray, out RaycastHit hit, 100f, furnitureMask))
        // {
        //     Debug.Log("hit1");
        //     FurniturePiece piece = hit.collider.GetComponent<FurniturePiece>();
        //     if (piece != null)
        //     {
        //         Debug.Log("hit2");
        //         FurnitureManager.Instance.Select(piece);

        //         FurnitureCopyButton.Instance.Show();

        //         if (FurnitureMoveGizmo.Instance != null)
        //             FurnitureMoveGizmo.Instance.AttachTo(piece);

        //         return;
        //     }
        // }

        // 빈 공간 클릭 → 선택 해제
        FurnitureManager.Instance.ClearSelection();
        FurnitureCopyButton.Instance.Hide();

        if (FurnitureMoveGizmo.Instance != null)
            FurnitureMoveGizmo.Instance.Detach();
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
