using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class MarkerPlacer : MonoBehaviour, IDropHandler
{
    [Header("3D 마커 프리팹 설정")]
    public GameObject marker3DPrefab;
    public LayerMask placementLayer;

    [Header("3D 위치 미리보기")]
    public GameObject marker3DGhostPrefab; // 고스트 프리팹
    private GameObject currentGhost;       // 현재 활성화된 고스트 오브젝트

    void Update()
    {
        // 마커 데이터가 드래그 중인 경우에만 동작
        if (UIMarkerItemData.markerDataToPlace != null && marker3DGhostPrefab != null)
        {
            // 1. 고스트가 없으면 생성
            if (currentGhost == null)
            {
                // 고스트 생성 (3D 마커와 별도로 이동됨)
                currentGhost = Instantiate(marker3DGhostPrefab, Vector3.zero, Quaternion.identity);
                currentGhost.name = "Ghost_Marker_Preview";

                SyncGhostVisuals(currentGhost, UIMarkerItemData.markerDataToPlace);
            }

            // 2. Raycast 처리 (OnDrop과 동일)
            Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, placementLayer))
            {
                // 3. 충돌 지점으로 고스트 이동 (높이 보정 포함)
                Renderer renderer = currentGhost.GetComponentInChildren<Renderer>();
                Vector3 finalPosition = hit.point;

                if (renderer != null)
                {
                    float offsetY = renderer.bounds.extents.y;
                    finalPosition = new Vector3(hit.point.x, hit.point.y + offsetY, hit.point.z);
                }

                // 4. 카메라를 바라보도록 회전 계산
                Transform mainCameraTransform = Camera.main.transform;

                // 마커에서 카메라를 향하는 방향 벡터
                Vector3 lookDirection = mainCameraTransform.position - finalPosition;

                // Y축 회전만 적용 (마커가 눕지 않도록)
                lookDirection.y = 0;

                Quaternion targetRotation = Quaternion.identity;

                if (lookDirection != Vector3.zero)
                {
                    // 마커의 정면(Z축)이 카메라를 향하도록 회전
                    targetRotation = Quaternion.LookRotation(lookDirection);
                    targetRotation *= Quaternion.Euler(0, 180, 0);
                }

                // 위치 및 회전 적용
                currentGhost.transform.position = finalPosition;
                currentGhost.transform.rotation = targetRotation;

                // 고스트 활성화
                if (!currentGhost.activeSelf)
                    currentGhost.SetActive(true);
            }
            else
            {
                // 충돌 지점이 없으면 고스트 숨김
                if (currentGhost.activeSelf)
                    currentGhost.SetActive(false);
            }
        }
        else if (currentGhost != null)
        {
            // 드래그가 끝나면 고스트 제거
            Destroy(currentGhost);
            currentGhost = null;
        }
    }

    // 드롭 이벤트 처리 (IDropHandler)
    public void OnDrop(PointerEventData eventData)
    {
        MarkerData dataToUse = UIMarkerItemData.markerDataToPlace;
        if (dataToUse == null) return;

        // 1. 1:1 대응되는 2D 마커 UI 요소 찾기
        UIMarkerItemData sourceUI = FindUIMarkerById(dataToUse.Id);
        if (sourceUI == null) return;

        // 2. Raycast로 배치 위치 계산
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, placementLayer))
        {
            GameObject targetMarker;
            bool isMovingExisting = (sourceUI.linked3DMarker != null);

            if (isMovingExisting)
            {
                // 이미 배치된 마커 이동
                targetMarker = sourceUI.linked3DMarker;
                Debug.Log($"[Placement] 마커 '{dataToUse.Name}'를 기존 위치로 이동합니다.");
            }
            else
            {
                // 새로운 마커 생성
                targetMarker = Instantiate(marker3DPrefab, hit.point, Quaternion.identity);

                // 특정 씬으로 이동
                Scene targetScene = SceneManager.GetSceneByName("Scene_3DSample");
                if (targetScene.isLoaded)
                {
                    SceneManager.MoveGameObjectToScene(targetMarker, targetScene);
                    targetMarker.name = "3D_Marker_" + dataToUse.Name;
                }

                // 2D UI와 3D 마커 연결
                sourceUI.linked3DMarker = targetMarker;
            }

            // 3. 높이 보정 후 위치 설정
            Renderer renderer = targetMarker.GetComponentInChildren<Renderer>();
            Vector3 finalPosition = hit.point;

            if (renderer != null)
            {
                float offsetY = renderer.bounds.extents.y;
                finalPosition = new Vector3(hit.point.x, hit.point.y + offsetY, hit.point.z);
            }

            // 카메라를 바라보도록 회전
            Transform mainCameraTransform = Camera.main.transform;
            Vector3 lookDirection = mainCameraTransform.position - finalPosition;
            lookDirection.y = 0;

            Quaternion targetRotation = Quaternion.identity;

            if (lookDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(lookDirection);
                targetRotation *= Quaternion.Euler(0, 180, 0);
            }

            targetMarker.transform.position = finalPosition;
            targetMarker.transform.rotation = targetRotation;

            // 4. ARMarkerData 초기화
            ARMarkerData arData = targetMarker.GetComponent<ARMarkerData>();
            if (arData != null)
            {
                arData.Initialize(dataToUse, targetMarker.transform.position, targetMarker.transform.rotation);
            }

            // 5. 시각 요소 동기화
            MarkerVisualSync visualSync = targetMarker.GetComponent<MarkerVisualSync>();
            if (visualSync != null)
                visualSync.UpdateVisuals();
        }
        else
        {
            Debug.LogWarning("Raycast 실패: 마커를 배치할 3D 표면을 찾지 못했습니다.");
        }

        // 드래그 상태 초기화
        UIMarkerItemData.markerDataToPlace = null;
    }

    private void SyncGhostVisuals(GameObject ghost, MarkerData data)
    {
        // 색상 동기화
        Renderer renderer = ghost.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Color newColor;
            if (ColorUtility.TryParseHtmlString(data.ColorCode, out newColor))
            {
                newColor.a = 0.3f; // 반투명
                renderer.material.color = newColor;
            }
        }

        // 텍스트 동기화 (3D TextMeshPro)
        TextMeshPro nameTag = ghost.GetComponentInChildren<TextMeshPro>();
        if (nameTag != null)
        {
            nameTag.text = data.Name;
        }
    }

    // Helper: ID로 2D 마커 UI 찾기
    private UIMarkerItemData FindUIMarkerById(string markerId)
    {
        UIMarkerItemData[] uiMarkers =
            FindObjectsByType<UIMarkerItemData>(FindObjectsSortMode.None);

        foreach (var uiMarker in uiMarkers)
        {
            if (uiMarker.Data != null && uiMarker.Data.Id == markerId)
                return uiMarker;
        }
        return null;
    }
}
