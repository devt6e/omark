using UnityEngine;
using UnityEngine.UI; // GridLayoutGroup 사용을 위해 추가
using System.Collections.Generic;
using TMPro;

public enum MarkerFilterMode { All, Bookmarked }

public class MarkerListUIController : MonoBehaviour
{
    // [UI 연결]
    [Header("UI 연결")]
    public Transform markerListContainer;
    public GameObject markerIconPrefab;

    [Header("뷰 제어")]
    public GameObject markerListPanel;

    [Header("패딩 조정 설정")]
    // Unity Inspector에서 Marker_Content에 붙어있는 GridLayoutGroup을 연결해야 합니다.
    public GridLayoutGroup markerContentGrid;
    private MarkerFilterMode currentFilterMode = MarkerFilterMode.All;

    public int threshold4Markers = 4; // 첫 번째 경계 (4개 미만)
    public int bookmark4Markers = 4;
    public int threshold8Markers = 8; // 두 번째 경계 (8개 미만)
    public int bookmark8Markers = 8;
    public int paddingLessThan4 = -300;      // 마커 수 < 4 일 때 적용
    public int padding4To7 = -150;           // 4 <= 마커 수 < 8 일 때 적용
    public int paddingGreaterEqual8 = 0;     // 마커 수 >= 8 일 때 적용

    private GameObject currentPlusButton;
    private List<GameObject> createdMarkerIcons = new List<GameObject>();
    public TextMeshProUGUI ListText;

    void Start()
    {
        Transform plusTransform = transform.Find("Plus");
        if (plusTransform != null)
        {
            currentPlusButton = plusTransform.gameObject;
        }
        else
        {
            Debug.LogError("Hierarchy에서 이름이 'Plus'인 객체를 찾을 수 없습니다. 수동 배치된 Plus 버튼을 확인하고 이름을 'Plus'로 지정하세요.");
        }

        // 초기 시작 시 기본 패딩 값 적용
        AdjustGridPadding();
    }

    // ======================================================================
    // 마커 리스트 필터링 함수 (추가)
    // ======================================================================
    public void SetMarkerFilter(MarkerFilterMode mode)
    {
        // 리스트 패널이 없으면 종료
        if (markerListPanel == null) return;

        currentFilterMode = mode;

        // 모든 생성된 마커 아이콘을 순회하며 상태를 확인합니다.
        foreach (GameObject markerIcon in createdMarkerIcons)
        {
            UIMarkerItemData uiItemData = markerIcon.GetComponent<UIMarkerItemData>();

            if (uiItemData != null && uiItemData.Data != null)
            {
                // 1. Plus 버튼은 필터링에서 제외하고 항상 보이게 합니다.
                if (uiItemData.Data.IsPlusButton) continue;

                bool shouldBeVisible = true;

                if (mode == MarkerFilterMode.Bookmarked)
                {
                    // 2. [즐겨찾기 모드]: IsFavorite이 true인 경우에만 shouldBeVisible = true
                    shouldBeVisible = uiItemData.Data.IsFavorite;
                }
                // else if mode == MarkerFilterMode.All, shouldBeVisible은 true를 유지

                // 3. GameObject의 활성화 상태를 업데이트합니다.
                markerIcon.SetActive(shouldBeVisible);
            }
        }
        AdjustGridPadding();
    }

    // ======================================================================
    // 마커 생성 시 호출되는 함수
    // ======================================================================
    public void UpdateInventoryDisplay(MarkerData newMarkerData)
    {
        if (currentPlusButton == null)
        {
            Debug.LogError("Plus 버튼 객체를 찾을 수 없어 갱신 로직을 실행할 수 없습니다.");
            return;
        }

        GameObject newMarkerIcon = Instantiate(
            markerIconPrefab,
            currentPlusButton.transform.position, // 위치를 복사
            Quaternion.identity,
            currentPlusButton.transform.parent // 부모를 복사
        );

        newMarkerIcon.transform.SetSiblingIndex(currentPlusButton.transform.GetSiblingIndex());

        // UIMarkerItemData 설정 로직 (이전 논의에서 구현됨)
        UIMarkerItemData uiItemData = newMarkerIcon.GetComponent<UIMarkerItemData>();
        if (uiItemData != null)
        {
            uiItemData.Setup(newMarkerData);
        }
        else
        {
            Debug.LogError("MarkerIconPrefab에 UIMarkerItemData 스크립트가 없습니다. 데이터를 저장할 수 없습니다.");
        }

        newMarkerIcon.name = newMarkerData.Name;
        createdMarkerIcons.Add(newMarkerIcon);
        if (currentFilterMode == MarkerFilterMode.Bookmarked && !newMarkerData.IsFavorite)
        {
            // 리스트가 '즐겨찾기만 보기' 모드이고, 새 마커가 즐겨찾기가 아니면 즉시 숨깁니다.
            newMarkerIcon.SetActive(false);
        }

        currentPlusButton.transform.SetAsLastSibling();

        // 마커가 추가되었으므로 패딩 조정
        AdjustGridPadding();
    }

    // ======================================================================
    // 마커 삭제 시 호출되는 함수 (UIPopupManager에서 호출)
    // ======================================================================
    public void RemoveMarkerIcon(string markerId)
    {
        GameObject markerToRemove = null;

        // 1. createdMarkerIcons 리스트를 순회하며 해당 ID를 가진 UI 오브젝트를 찾습니다.
        foreach (GameObject markerIcon in createdMarkerIcons)
        {
            UIMarkerItemData uiItemData = markerIcon.GetComponent<UIMarkerItemData>();

            if (uiItemData != null && uiItemData.Data.Id == markerId)
            {
                markerToRemove = markerIcon;

                // **[핵심]** 연결된 3D 마커를 찾아 파괴
                if (uiItemData.linked3DMarker != null)
                {
                    Destroy(uiItemData.linked3DMarker);
                    Debug.Log($"[Delete] 연결된 3D 마커({markerId})를 파괴했습니다.");
                }

                break;
            }
        }

        if (markerToRemove != null)
        {
            // 2. 리스트에서 제거
            createdMarkerIcons.Remove(markerToRemove);

            // 3. 씬에서 오브젝트 파괴
            Destroy(markerToRemove);

            // 마커가 제거되었으므로 패딩 조정
            AdjustGridPadding();

            Debug.Log($"[UI List] 마커 ID {markerId}의 UI 항목이 리스트에서 제거되었습니다.");
        }
    }

    // ======================================================================
    // 마커 개수에 따라 Grid Layout Group의 패딩을 조정하는 핵심 함수
    // ======================================================================
    private void AdjustGridPadding()
    {
        if (markerContentGrid == null) return;

        int count; // 기준이 될 마커 개수
        int threshold4, threshold8; // 기준이 될 경계 값

        if (currentFilterMode == MarkerFilterMode.All)
        {
            // [전체보기 모드]: createdMarkerIcons 리스트의 총 개수 사용
            count = createdMarkerIcons.Count;
            threshold4 = threshold4Markers; // 예: 4
            threshold8 = threshold8Markers; // 예: 8
            Debug.Log($"[패딩] 전체보기 모드: 마커 수 {count}개.");
        }
        else // MarkerFilterMode.Bookmarked
        {
            // [즐겨찾기 모드]: IsFavorite이 true인 마커의 개수만 카운트
            count = 0;
            foreach (var icon in createdMarkerIcons)
            {
                var data = icon.GetComponent<UIMarkerItemData>()?.Data;
                if (data != null && data.IsFavorite && !data.IsPlusButton)
                {
                    count++;
                }
            }
            threshold4 = bookmark4Markers; // 예: 4
            threshold8 = bookmark8Markers; // 예: 8
            Debug.Log($"[패딩] 즐겨찾기 모드: 즐겨찾기 수 {count}개.");
        }

        // Grid Layout Group의 Padding 구조체 복사
        GridLayoutGroup grid = markerContentGrid;
        RectOffset padding = grid.padding;
        int newPaddingTop;

        // count와 동적으로 선택된 threshold 값을 사용하여 패딩 계산
        if (count < threshold4) // 마커 수 < 4
        {
            newPaddingTop = paddingLessThan4; // 예: -300
        }
        else if (count < threshold8) // 4 <= 마커 수 < 8
        {
            newPaddingTop = padding4To7; // 예: -150
        }
        else // 마커 수 >= 8
        {
            newPaddingTop = paddingGreaterEqual8; // 예: 0
        }

        if (padding.top != newPaddingTop)
        {
            padding.top = newPaddingTop;

            // 수정된 Padding 구조체를 다시 컴포넌트에 적용
            grid.padding = padding;
            Debug.Log($"[패딩] 최종 Top Padding을 {newPaddingTop}으로 설정.");
        }
    }

    // ======================================================================
    // 마커 편집 시 UI 갱신을 위해 호출되는 함수 (UIPopupManager에서 사용)
    // ======================================================================
    public void UpdateMarkerIconStatus(MarkerData updatedData)
    {
        foreach (GameObject markerIcon in createdMarkerIcons)
        {
            UIMarkerItemData uiItemData = markerIcon.GetComponent<UIMarkerItemData>();

            if (uiItemData != null && uiItemData.Data.Id == updatedData.Id)
            {
                // UIMarkerItemData의 Setup 함수를 호출하여 이름, 색상, 즐겨찾기 상태 갱신
                uiItemData.Setup(updatedData);
                markerIcon.name = updatedData.Name; // GameObject 이름도 갱신

                // 2. 3D 마커 비주얼 업데이트 요청**
                if (uiItemData.linked3DMarker != null)
                {
                    // 3D 마커 오브젝트에서 MarkerVisualSync 컴포넌트를 가져옵니다.
                    MarkerVisualSync visualSync = uiItemData.linked3DMarker.GetComponent<MarkerVisualSync>();

                    if (visualSync != null)
                    {
                        // 3D 마커의 비주얼 갱신 함수를 명시적으로 호출
                        visualSync.UpdateVisuals();
                        Debug.Log($"[Sync] 3D 마커 '{updatedData.Name}' 비주얼이 즉시 갱신되었습니다.");
                    }
                }
                // 3. 데이터 갱신 후, 현재 필터 모드를 다시 적용
                SetMarkerFilter(currentFilterMode);
                return; // 갱신 완료
            }
        }
    }

    // 마커 리스트 패널의 활성화/비활성화를 제어하는 함수 (슬라이더 로직에서 사용)
    public void SetPanelVisibility(bool isVisible)
    {
        if (markerListPanel != null)
        {
            markerListPanel.SetActive(isVisible);
        }
    }

    public void OnBookmarkFilterToggled(bool isOn)
    {
        if (isOn)
        {
            // 토글이 켜졌을 때 (Checked) -> 즐겨찾기만 보기
            SetMarkerFilter(MarkerFilterMode.Bookmarked);
            ListText.text = "즐겨찾기";
        }
        else
        {
            // 토글이 꺼졌을 때 (Unchecked) -> 전체 보기
            SetMarkerFilter(MarkerFilterMode.All);
            ListText.text = "마커리스트";
        }
    }
}