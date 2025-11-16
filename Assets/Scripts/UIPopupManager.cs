using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 필요합니다.

public class UIPopupManager : MonoBehaviour
{
    // ======================================================================
    // 1. 메인 팝업 및 패널 연결 (Unity Inspector에 연결)
    // ======================================================================
    [Header("1. 메인 팝업 및 패널")]
    public GameObject markerDetailPopup;    // 마커 상세 정보 팝업 (SCENE3-3 초기 화면)
    public GameObject deleteConfirmPopup;   // "정말 삭제하시겠습니까?" 패널 (삭제 확인)
    public GameObject detailEditPanel;      // 편집 상세 정보 입력 패널

    // ======================================================================
    // 2. 메인 팝업 UI 요소 연결
    // ======================================================================
    [Header("2. 메인 팝업 내부 요소")]
    public TextMeshProUGUI nameText;        // 마커 이름 표시
    public TextMeshProUGUI detailInfoText;  // 상세 정보 표시

    // 북마크 (즐겨찾기) 상태 표시 및 제어
    public Button bookmarkButton;           // 즐겨찾기(Bookmark) 버튼
    public Image bookmarkImageComponent;     // 즐겨찾기 버튼의 Image 컴포넌트
    public Sprite filledStarSprite;           // 노란색 채워진 별 이미지 (즐겨찾기 O)
    public Sprite emptyStarSprite;            // 빈 별 이미지 (즐겨찾기 X)

    public Button deleteButton;             // '삭제' 버튼 (-> 삭제 확인 팝업)
    public Button editButton;               // '편집' 버튼 (-> 편집 상세 패널)
    public Button closeButton;              // 닫기/취소 버튼

    // ======================================================================
    // 3. 편집 패널 UI 요소 연결
    // ======================================================================
    [Header("3. 편집 패널 내부 요소")]
    public TMP_InputField nameInputField;   // **(필수) 마커 이름 입력 필드**
    public TMP_InputField detailInputField; // **(필수) 새로 추가된 상세 정보 입력 필드**
    public GameObject colorButtonContainer; // 색상 버튼들을 담는 부모 오브젝트 (선택적)
    public Button editConfirmButton;        // 편집 패널의 '확인' 버튼
    public Button editCancelButton;         // 편집 패널의 '취소' 버튼
    // 참고: 색상 버튼들은 개별적으로 이 스크립트의 OnColorSelected 함수에 연결해야 합니다.

    // ======================================================================
    // 4. 삭제 확인 팝업 UI 요소 연결
    // ======================================================================
    [Header("4. 삭제 확인 팝업 내부 요소")]
    public Button deleteYesButton;          // 삭제 확인 팝업의 '예' 버튼
    public Button deleteNoButton;           // 삭제 확인 팝업의 '아니오' 버튼

    // ======================================================================
    // 5. 내부 상태 변수
    // ======================================================================
    private MarkerData currentMarkerData;    // 현재 팝업에 표시 중인 마커 데이터
    private string selectedColorCode;       // 편집 중 선택된 색상
    private bool tempIsFavorite;
    private ColorButtonTag[] colorButtons;

    void Start()
    {
        // 1. 모든 패널 시작 시 숨기기
        if (markerDetailPopup != null) markerDetailPopup.SetActive(false);
        if (deleteConfirmPopup != null) deleteConfirmPopup.SetActive(false);
        if (detailEditPanel != null) detailEditPanel.SetActive(false);

        // 2. 메인 팝업 버튼 연결
        if (bookmarkButton != null) bookmarkButton.onClick.AddListener(OnBookmarkToggled);
        if (deleteButton != null) deleteButton.onClick.AddListener(OnDeleteClicked);
        if (editButton != null) editButton.onClick.AddListener(OnEditClicked);
        if (closeButton != null) closeButton.onClick.AddListener(HideMarkerDetailPopup);

        // 3. 편집 패널 버튼 연결
        if (editConfirmButton != null) editConfirmButton.onClick.AddListener(OnEditConfirmed);
        if (editCancelButton != null) editCancelButton.onClick.AddListener(OnEditCancelled);

        // 4. 삭제 확인 팝업 버튼 연결
        if (deleteYesButton != null) deleteYesButton.onClick.AddListener(OnDeleteFinalConfirmed);
        if (deleteNoButton != null) deleteNoButton.onClick.AddListener(HideDeleteConfirmPopup);

        // 초기 색상 설정 (예시)
        selectedColorCode = "#FFFFFF";
    }

    // ======================================================================
    // 마커 상세 팝업 표시/숨기기 (Marker Icon Clicked)
    // ======================================================================

    // UIMarkerItemData.cs에서 호출하는 함수
    public void ShowMarkerDetailPopup(MarkerData data)
    {
        currentMarkerData = data;
        tempIsFavorite = data.IsFavorite;

        // 팝업에 데이터 반영
        if (nameText != null)
        {
            nameText.text = data.Name;
        }

        if (detailInfoText != null)
        {
            // DetailInformation이 null이거나 비어있으면 기본값(예: "설명 없음")을 표시할 수 있습니다.
            detailInfoText.text = string.IsNullOrEmpty(data.DetailInformation)
                                  ? "마커 상세 설명이 없습니다."
                                  : data.DetailInformation;
        }

        UpdateBookmarkVisual(tempIsFavorite);

        // 팝업 표시
        if (markerDetailPopup != null)
        {
            markerDetailPopup.SetActive(true);
        }
        Debug.Log($"[Popup] 상세 팝업 표시. 마커 ID: {data.Id}");
    }

    private void UpdateBookmarkVisual(bool isFavorite)
    {
        if (bookmarkImageComponent != null && filledStarSprite != null && emptyStarSprite != null)
        {
            if (isFavorite)
            {
                bookmarkImageComponent.sprite = filledStarSprite;
            }
            else
            {
                bookmarkImageComponent.sprite = emptyStarSprite;
            }
        }
        else
        {
            Debug.LogWarning("즐겨찾기 이미지 컴포넌트 또는 스프라이트 연결이 누락되었습니다.");
        }
    }

    public void HideMarkerDetailPopup()
    {
        if (markerDetailPopup != null)
        {
            markerDetailPopup.SetActive(false);
        }
        currentMarkerData = null;
    }

    // ======================================================================
    // 1. 즐겨찾기 (Bookmark) 토글 로직
    // ======================================================================
    private void OnBookmarkToggled()
    {
        if (currentMarkerData != null)
        {
            // 1. 데이터 업데이트
            tempIsFavorite = !tempIsFavorite;

            // 2. 팝업 UI 반영 (별 이미지 즉시 변경)
            UpdateBookmarkVisual(tempIsFavorite);

            Debug.Log($"[Action] 즐겨찾기 상태 변경: ID {currentMarkerData.Id} -> {currentMarkerData.IsFavorite}");
        }
    }

    // ======================================================================
    // 2. 삭제 로직 (Delete Button)
    // ======================================================================

    // 메인 팝업에서 '삭제' 버튼 클릭 시 (삭제 확인 패널 요청)
    private void OnDeleteClicked()
    {
        if (currentMarkerData != null && deleteConfirmPopup != null)
        {
            // "정말 삭제하시겠습니까?" 패널 띄우기
            deleteConfirmPopup.SetActive(true);
            //HideMarkerDetailPopup(); // 상세 정보 팝업
        }
    }

    // 삭제 확인 패널에서 '예' 버튼 클릭 시 (최종 삭제 실행)
    private void OnDeleteFinalConfirmed()
    {
        if (currentMarkerData != null)
        {
            string deletedId = currentMarkerData.Id;

            Debug.Log($"[Action] 최종 마커 삭제 실행: ID {deletedId}");

            // TODO: 1. 서버에 삭제 요청
            // TODO: 2. AR 공간에서 3D 마커 오브젝트 파괴

            // 3. UI 리스트에서 마커 아이콘 오브젝트 제거 (추가된 핵심 로직)
            MarkerListUIController uiController = FindFirstObjectByType<MarkerListUIController>();
            if (uiController != null)
            {
                uiController.RemoveMarkerIcon(deletedId);
            }

            currentMarkerData = null;
            HideDeleteConfirmPopup();
            HideMarkerDetailPopup();
        }
    }

    // 삭제 확인 패널 닫기 (취소/아니오 버튼 클릭 시)
    public void HideDeleteConfirmPopup()
    {
        if (deleteConfirmPopup != null)
        {
            deleteConfirmPopup.SetActive(false);
        }
    }

    // ======================================================================
    // 3. 편집 로직 (Edit Button)
    // ======================================================================

    private void UpdateColorSelectionVisual()
    {
        if (colorButtons == null) return;

        foreach (var tag in colorButtons)
        {
            if (tag.selectionHighlight != null)
            {
                // 현재 선택된 색상 코드와 버튼의 색상 코드가 일치하는지 확인
                bool isSelected = tag.buttonColorCode == selectedColorCode;

                // 일치하면 강조 오브젝트를 켜고, 아니면 끕니다.
                tag.selectionHighlight.SetActive(isSelected);

                Debug.Log($"Checking {tag.buttonColorCode}. Selected: {isSelected}");
            }
        }
    }

    // 메인 팝업에서 '편집' 버튼 클릭 시
    private void OnEditClicked()
    {
        if (currentMarkerData != null && detailEditPanel != null)
        {
            // 1. 편집 패널에 현재 이름 데이터 미리 채우기
            if (nameInputField != null)
            {
                nameInputField.text = currentMarkerData.Name;
            }

            // 2. **상세 정보 데이터 미리 채우기 (추가)**
            if (detailInputField != null)
            {
                detailInputField.text = currentMarkerData.DetailInformation;
            }

            // 3. 색상 초기화
            selectedColorCode = currentMarkerData.ColorCode;
            // colorButtonContainer 하위의 모든 ColorButtonTag 컴포넌트를 가져옵니다.
            if (colorButtonContainer != null)
            {
                colorButtons = colorButtonContainer.GetComponentsInChildren<ColorButtonTag>();
            }
            UpdateColorSelectionVisual();
            // TODO: 기존 색상 버튼이 선택된 상태로 시각적으로 표시되도록 하는 로직 추가

            // 4. 패널 표시
            detailEditPanel.SetActive(true);
            //HideMarkerDetailPopup(); // 상세 정보 팝업은 숨김
        }
    }

    // 편집 패널 내 색상 버튼 클릭 시 호출 (각 색상 버튼의 OnClick에 연결)
    public void OnColorSelected(string colorCode)
    {
        selectedColorCode = colorCode;

        UpdateColorSelectionVisual();

        Debug.Log($"[Edit] 새 색상 선택: {colorCode}");
    }

    // 편집 패널의 '확인' 버튼 클릭 시 (편집 저장)
    private void OnEditConfirmed()
    {
        if (currentMarkerData != null)
        {
            currentMarkerData.IsFavorite = tempIsFavorite;

            MarkerListUIController uiController = FindFirstObjectByType<MarkerListUIController>();
            if (uiController != null)
            {
                // UpdateMarkerIconStatus 내부에서 갱신된 currentMarkerData를 사용합니다.
                uiController.UpdateMarkerIconStatus(currentMarkerData);
            }

            // 1. 이름 변경 (Detail information 입력)
            string newName = nameInputField.text;
            if (nameInputField != null && !string.IsNullOrEmpty(newName))
            {
                currentMarkerData.Name = newName;
            }

            // 2. **상세 정보 저장 (추가)**
            string newDetail = detailInputField.text;
            if (detailInputField != null) // 비어 있어도 저장 (사용자가 지울 수 있음)
            {
                currentMarkerData.DetailInformation = newDetail;
            }

            // 3. 색상 변경
            if (!string.IsNullOrEmpty(selectedColorCode))
            {
                currentMarkerData.ColorCode = selectedColorCode;
            }

            // TODO: 서버에 업데이트 요청 및 UI 리스트 갱신 요청

            // 4. **UI 리스트 갱신 요청 (핵심 누락/오류 지점)**
            if (uiController != null)
            {
                uiController.UpdateMarkerIconStatus(currentMarkerData); // currentMarkerData를 전달하여 리스트 UI의 이름, 색상 등을 갱신합니다.
            }

            Debug.Log($"[Action] 편집 저장 완료. Name: {currentMarkerData.Name}, Detail: {currentMarkerData.DetailInformation}");
            HideMarkerDetailPopup();
            HideEditPanel(); // 편집 패널 닫기
        }
    }

    // 편집 패널의 '취소' 버튼 클릭 시 (마커 편집 리스트로 돌아옴)
    private void OnEditCancelled()
    {
        // 편집을 취소하고 편집 패널을 닫습니다.
        HideEditPanel();
    }

    public void HideEditPanel()
    {
        if (detailEditPanel != null)
        {
            detailEditPanel.SetActive(false);
        }
    }
}