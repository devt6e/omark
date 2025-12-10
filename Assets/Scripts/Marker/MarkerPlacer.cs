using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class MarkerPlacer : MonoBehaviour, IDropHandler
{
    [Header("3D ������Ʈ ����")]
    public GameObject marker3DPrefab;
    public LayerMask placementLayer;

    [Header("3D ��ġ �̸�����")]
    public GameObject marker3DGhostPrefab; // ����Ʈ ������
    private GameObject currentGhost;       // ���� ���� �ִ� ����Ʈ ������Ʈ

    void Update()
    {
        // ��Ŀ �����Ͱ� �巡�� ���� ��쿡�� �۵�
        if (UIMarkerItemData.markerDataToPlace != null && marker3DGhostPrefab != null)
        {
            // 1. ����Ʈ�� ������ ����
            if (currentGhost == null)
            {
                // ����Ʈ ���� (3D ������ �ٷ� �̵���ų �ʿ� ����)
                currentGhost = Instantiate(marker3DGhostPrefab, Vector3.zero, Quaternion.identity);
                currentGhost.name = "Ghost_Marker_Preview";

                SyncGhostVisuals(currentGhost, UIMarkerItemData.markerDataToPlace);
            }

            // 2. Raycast ���� (OnDrop�� ����)
            Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, placementLayer))
            {
                // 3. �浹 ������ ����Ʈ �̵� (���� �ڵ� ���� ����)
                Renderer renderer = currentGhost.GetComponentInChildren<Renderer>();
                Vector3 finalPosition = hit.point;

                if (renderer != null)
                {
                    float offsetY = renderer.bounds.extents.y;
                    finalPosition = new Vector3(hit.point.x, hit.point.y + offsetY, hit.point.z);
                }

                // 2. **[�ٽ� �߰�] ī�޶� ���� ȸ�� ���**
                Transform mainCameraTransform = Camera.main.transform;

                // ��Ŀ���� ī�޶� ���ϴ� ���� ���� ���
                Vector3 lookDirection = mainCameraTransform.position - finalPosition;

                // Y�� ������ 0���� ����� ���� ȸ���� ���� (��Ŀ�� �������� �� ����)
                lookDirection.y = 0;

                Quaternion targetRotation = Quaternion.identity;

                if (lookDirection != Vector3.zero)
                {
                    // LookRotation: ��Ŀ�� ����(Z��)�� lookDirection�� ���ϵ��� ȸ�� �� ����
                    targetRotation = Quaternion.LookRotation(lookDirection);
                    targetRotation *= Quaternion.Euler(0, 180, 0);
                }

                // 3. ��ġ �� ȸ�� ����
                currentGhost.transform.position = finalPosition;
                currentGhost.transform.rotation = targetRotation; // **<- ȸ�� ����**

                // 4. ����Ʈ�� Ȱ��ȭ (���̰�)
                if (!currentGhost.activeSelf) currentGhost.SetActive(true);
            }
            else
            {
                // �浹 ǥ���� ������ ����Ʈ ����
                if (currentGhost.activeSelf) currentGhost.SetActive(false);
            }
        }
        else if (currentGhost != null)
        {
            // �巡�װ� �������� ����Ʈ �ı�
            Destroy(currentGhost);
            currentGhost = null;
        }
    }

    // ��� �̺�Ʈ ���� (IDropHandler)
    public void OnDrop(PointerEventData eventData)
    {
        MarkerData dataToUse = UIMarkerItemData.markerDataToPlace;

        if (dataToUse == null) return;

        // 1. **[1:1 ���� ����]** 2D ��Ŀ UI �׸��� ã���ϴ�.
        UIMarkerItemData sourceUI = FindUIMarkerById(dataToUse.Id);
        if (sourceUI == null) return; // 2D source UI�� ã�� ���ϸ� ����

        // 2. Raycast �غ� �� ���� (���ο� ��ġ ��ġ ã��)
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, placementLayer))
        {
            GameObject targetMarker; // ���� �Ǵ� �̵��� 3D ��Ŀ
            bool isMovingExisting = (sourceUI.linked3DMarker != null);

            if (isMovingExisting)
            {
                // === PATH 1: �̹� ��ġ�� ��Ŀ�� �̵� (��û�Ͻ� ���) ===
                targetMarker = sourceUI.linked3DMarker;
                Debug.Log($"[Placement] ��Ŀ '{dataToUse.Name}'�� �� ��ġ�� �̵��մϴ�.");
            }
            else
            {
                // === PATH 2: ���ο� ��Ŀ ���� ===
                targetMarker = Instantiate(marker3DPrefab, hit.point, Quaternion.identity);

                // �� �̵� (���� ������ ��Ŀ���� �ʿ�)
                Scene targetScene = SceneManager.GetSceneByName("Scene_3DSample");
                if (targetScene.isLoaded)
                {
                    SceneManager.MoveGameObjectToScene(targetMarker, targetScene);
                    targetMarker.name = "3D_Marker_" + dataToUse.Name;
                }

                // 2D ��Ŀ�� 3D ������Ʈ ���� ����
                sourceUI.linked3DMarker = targetMarker;
            }

            // 3. **[�ٽ�] ���� �ڵ� ���� �� ��ġ ����**
            Renderer renderer = targetMarker.GetComponentInChildren<Renderer>();
            Vector3 finalPosition = hit.point;

            if (renderer != null)
            {
                // ��Ŀ ������ ����(extents.y)�� ����Ͽ� Y ��ġ ����
                float offsetY = renderer.bounds.extents.y;
                finalPosition = new Vector3(hit.point.x, hit.point.y + offsetY, hit.point.z);
            }

            // ī�޶� �ٶ󺸵��� ȸ�� ����
            Transform mainCameraTransform = Camera.main.transform;
            Vector3 lookDirection = mainCameraTransform.position - finalPosition;
            lookDirection.y = 0; // Y�� ����

            Quaternion targetRotation = Quaternion.identity;

            if (lookDirection != Vector3.zero)
            {
                // LookRotation: ��Ŀ�� ����(Z��)�� lookDirection�� ���ϵ��� ȸ�� �� ����
                targetRotation = Quaternion.LookRotation(lookDirection);
                targetRotation *= Quaternion.Euler(0, 180, 0); 
            }

            targetMarker.transform.position = finalPosition;
            targetMarker.transform.rotation = targetRotation;

            // 4. ������ ������Ʈ (��ġ�� ����Ǿ����Ƿ� ARMarkerData ����)
            ARMarkerData arData = targetMarker.GetComponent<ARMarkerData>();
            if (arData != null)
            {
                // fullMarkerData�� �����մϴ�.
                arData.Initialize(dataToUse, targetMarker.transform.position, targetMarker.transform.rotation);
            }
            // 5. ���־� ������Ʈ �� ��Ŀ�� (MarkerVisualSync �� CameraFocusController ȣ��)
            MarkerVisualSync visualSync = targetMarker.GetComponent<MarkerVisualSync>();
            if (visualSync != null) { visualSync.UpdateVisuals(); }
        }
        else
        {
            Debug.LogWarning("Raycast �浹 ����: ��Ŀ�� ��ġ�� 3D ǥ���� ã�� �� �����ϴ�. (�̵� ����)");
        }

        // 5. ��� �� ������ �ʱ�ȭ
        UIMarkerItemData.markerDataToPlace = null;
    }

    private void SyncGhostVisuals(GameObject ghost, MarkerData data)
    {
        // 1. Color Synchronization
        Renderer renderer = ghost.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Color newColor;
            if (ColorUtility.TryParseHtmlString(data.ColorCode, out newColor))
            {
                // Material�� Alpha ���� ���߾� �������ϰ� ����ϴ�. (Alpha 0.3)
                newColor.a = 0.3f;
                renderer.material.color = newColor;
            }
        }

        // 2. Text Synchronization (3D TextMeshPro ��� ����)
        TextMeshPro nameTag = ghost.GetComponentInChildren<TextMeshPro>();
        if (nameTag != null)
        {
            nameTag.text = data.Name;
        }
    }

    // Helper �Լ� (ID�� 2D ��Ŀ ã��)
    private UIMarkerItemData FindUIMarkerById(string markerId)
    {
        // �� ��ü���� UIMarkerItemData ������Ʈ�� ���� ������Ʈ�� ã���ϴ�.
        UIMarkerItemData[] uiMarkers = FindObjectsByType<UIMarkerItemData>(FindObjectsSortMode.None);

        foreach (var uiMarker in uiMarkers)
        {
            if (uiMarker.Data != null && uiMarker.Data.Id == markerId)
            {
                return uiMarker;
            }
        }
        return null;
    }
}