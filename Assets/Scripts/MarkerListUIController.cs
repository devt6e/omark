using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MarkerListUIController : MonoBehaviour
{
    // [UI 연결]
    [Header("UI 연결")]
    public Transform markerListContainer;
    public GameObject markerIconPrefab;

    [Header("뷰 제어")]
    public GameObject markerListPanel;

    private GameObject currentPlusButton;

    private List<GameObject> createdMarkerIcons = new List<GameObject>();

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
    }

    public void UpdateMarkerIconStatus(MarkerData updatedData)
    {
        // 리스트를 순회하며 해당 ID를 가진 UI 오브젝트를 찾습니다.
        foreach (GameObject markerIcon in createdMarkerIcons)
        {
            // UI 오브젝트에 붙어있는 UIMarkerItemData 스크립트에서 ID를 가져옵니다.
            UIMarkerItemData uiItemData = markerIcon.GetComponent<UIMarkerItemData>();

            if (uiItemData != null && uiItemData.Data.Id == updatedData.Id)
            {
                // 1. 저장된 데이터 업데이트 (선택 사항이지만 안전을 위해)
                // uiItemData.Data는 참조이므로 이미 업데이트되었을 수 있습니다.
                // 하지만 시각적 갱신을 위해 Setup을 다시 호출합니다.

                // 2. UI 시각적 갱신: 즐겨찾기 별 이미지 등을 다시 설정합니다.
                uiItemData.Setup(updatedData);

                Debug.Log($"[UI List] 마커 ID {updatedData.Id}의 UI 상태가 갱신되었습니다.");
                return;
            }
        }
    }

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

        UIMarkerItemData uiItemData = newMarkerIcon.GetComponent<UIMarkerItemData>();
        if (uiItemData != null)
        {
            uiItemData.Setup(newMarkerData); // 데이터를 UI 컴포넌트에 영구적으로 저장
        }
        else
        {
            Debug.LogError("MarkerIconPrefab에 UIMarkerItemData 스크립트가 없습니다. 데이터를 저장할 수 없습니다.");
        }

        newMarkerIcon.name = newMarkerData.Name;
        createdMarkerIcons.Add(newMarkerIcon);

        currentPlusButton.transform.SetAsLastSibling();

    }

    public void SetPanelVisibility(bool isVisible)
    {
        if (markerListPanel != null)
        {
            markerListPanel.SetActive(isVisible);
        }
    }

    public void RemoveMarkerIcon(string markerId)
    {
        GameObject markerToRemove = null;

        // 1. createdMarkerIcons 리스트를 순회하며 해당 ID를 가진 UI 오브젝트를 찾습니다.
        foreach (GameObject markerIcon in createdMarkerIcons)
        {
            // UI 오브젝트에 붙어있는 UIMarkerItemData 스크립트에서 ID를 가져옵니다.
            UIMarkerItemData uiItemData = markerIcon.GetComponent<UIMarkerItemData>();

            if (uiItemData != null && uiItemData.Data.Id == markerId)
            {
                markerToRemove = markerIcon;
                break;
            }
        }

        if (markerToRemove != null)
        {
            // 2. 리스트에서 제거
            createdMarkerIcons.Remove(markerToRemove);

            // 3. 씬에서 오브젝트 파괴 (UI에서 사라지게 함)
            Destroy(markerToRemove);

            Debug.Log($"[UI List] 마커 ID {markerId}의 UI 항목이 리스트에서 제거되었습니다.");
        }
        else
        {
            Debug.LogWarning($"[UI List] 마커 ID {markerId}를 리스트에서 찾을 수 없습니다.");
        }
    }
}