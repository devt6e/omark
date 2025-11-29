using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class MarkerPlacer : MonoBehaviour, IDropHandler
{
    [Header("3D 오브젝트 설정")]
    public GameObject marker3DPrefab;
    public LayerMask placementLayer;

    [Header("3D 배치 미리보기")]
    public GameObject marker3DGhostPrefab; // 고스트 프리팹
    private GameObject currentGhost;       // 현재 씬에 있는 고스트 오브젝트

    void Update()
    {
        // 마커 데이터가 드래그 중인 경우에만 작동
        if (UIMarkerItemData.markerDataToPlace != null && marker3DGhostPrefab != null)
        {
            // 1. 고스트가 없으면 생성
            if (currentGhost == null)
            {
                // 고스트 생성 (3D 씬으로 바로 이동시킬 필요 없음)
                currentGhost = Instantiate(marker3DGhostPrefab, Vector3.zero, Quaternion.identity);
                currentGhost.name = "Ghost_Marker_Preview";

                SyncGhostVisuals(currentGhost, UIMarkerItemData.markerDataToPlace);
            }

            // 2. Raycast 수행 (OnDrop과 동일)
            Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, placementLayer))
            {
                // 3. 충돌 지점에 고스트 이동 (높이 자동 조정 포함)
                Renderer renderer = currentGhost.GetComponentInChildren<Renderer>();
                Vector3 finalPosition = hit.point;

                if (renderer != null)
                {
                    float offsetY = renderer.bounds.extents.y;
                    finalPosition = new Vector3(hit.point.x, hit.point.y + offsetY, hit.point.z);
                }

                currentGhost.transform.position = finalPosition;

                // 4. 고스트를 활성화 (보이게)
                currentGhost.SetActive(true);
            }
            else
            {
                // 충돌 표면이 없으면 고스트 숨김
                if (currentGhost.activeSelf) currentGhost.SetActive(false);
            }
        }
        else if (currentGhost != null)
        {
            // 드래그가 끝났으면 고스트 파괴
            Destroy(currentGhost);
            currentGhost = null;
        }
    }

    // 드롭 이벤트 수신 (IDropHandler)
    public void OnDrop(PointerEventData eventData)
    {
        MarkerData dataToUse = UIMarkerItemData.markerDataToPlace;

        if (dataToUse == null) return;

        // 1. **[1:1 생성 제어]** 2D 마커 UI 항목을 찾습니다.
        UIMarkerItemData sourceUI = FindUIMarkerById(dataToUse.Id);
        if (sourceUI == null) return; // 2D source UI를 찾지 못하면 종료

        // 2. Raycast 준비 및 실행 (새로운 배치 위치 찾기)
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, placementLayer))
        {
            GameObject targetMarker; // 생성 또는 이동될 3D 마커
            bool isMovingExisting = (sourceUI.linked3DMarker != null);

            if (isMovingExisting)
            {
                // === PATH 1: 이미 배치된 마커를 이동 (요청하신 기능) ===
                targetMarker = sourceUI.linked3DMarker;
                Debug.Log($"[Placement] 마커 '{dataToUse.Name}'을 새 위치로 이동합니다.");
            }
            else
            {
                // === PATH 2: 새로운 마커 생성 ===
                targetMarker = Instantiate(marker3DPrefab, hit.point, Quaternion.identity);

                // 씬 이동 (새로 생성된 마커에만 필요)
                Scene targetScene = SceneManager.GetSceneByName("Scene_3DSample");
                if (targetScene.isLoaded)
                {
                    SceneManager.MoveGameObjectToScene(targetMarker, targetScene);
                    targetMarker.name = "3D_Marker_" + dataToUse.Name;
                }

                // 2D 마커에 3D 오브젝트 참조 저장
                sourceUI.linked3DMarker = targetMarker;
            }

            // 3. **[핵심] 높이 자동 조정 및 위치 적용**
            Renderer renderer = targetMarker.GetComponentInChildren<Renderer>();
            Vector3 finalPosition = hit.point;

            if (renderer != null)
            {
                // 마커 높이의 절반(extents.y)을 계산하여 Y 위치 조정
                float offsetY = renderer.bounds.extents.y;
                finalPosition = new Vector3(hit.point.x, hit.point.y + offsetY, hit.point.z);
            }

            // 최종 위치/회전 적용
            targetMarker.transform.position = finalPosition;
            targetMarker.transform.rotation = Quaternion.identity; // 기본 회전으로 설정

            // 4. 데이터 업데이트 (위치가 변경되었으므로 ARMarkerData 갱신)
            ARMarkerData arData = targetMarker.GetComponent<ARMarkerData>();
            if (arData != null)
            {
                // **수정:** fullMarkerData를 전달합니다.
                arData.Initialize(dataToUse, targetMarker.transform.position, targetMarker.transform.rotation);

                // **[핵심 추가]** 비주얼 동기화 호출
                MarkerVisualSync visualSync = targetMarker.GetComponent<MarkerVisualSync>();
                if (visualSync != null)
                {
                    visualSync.UpdateVisuals();
                }
            }
        }
        else
        {
            Debug.LogWarning("Raycast 충돌 없음: 마커를 배치할 3D 표면을 찾을 수 없습니다. (이동 실패)");
        }

        // 5. 사용 후 데이터 초기화
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
                // Material의 Alpha 값만 낮추어 반투명하게 만듭니다. (Alpha 0.3)
                newColor.a = 0.3f;
                renderer.material.color = newColor;
            }
        }

        // 2. Text Synchronization (3D TextMeshPro 사용 가정)
        TextMeshPro nameTag = ghost.GetComponentInChildren<TextMeshPro>();
        if (nameTag != null)
        {
            nameTag.text = data.Name;
        }
    }

    // Helper 함수 (ID로 2D 마커 찾기)
    private UIMarkerItemData FindUIMarkerById(string markerId)
    {
        // 씬 전체에서 UIMarkerItemData 컴포넌트를 가진 오브젝트를 찾습니다.
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