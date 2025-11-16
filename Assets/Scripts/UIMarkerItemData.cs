using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용한다면 필요합니다. (Unity Text 사용 시 제거)

public class UIMarkerItemData : MonoBehaviour
{
    // 이 UI 항목이 가지는 MarkerData 객체 전체를 참조합니다. (데이터 저장 위치)
    private MarkerData _data;
    public MarkerData Data { get { return _data; } }
    private float lastClickTime = 0f;
    private const float DOUBLE_CLICK_TIME = 0.3f;

    [Header("UI 요소 연결")]
    public TextMeshProUGUI nameText; // 마커 이름을 표시하는 Text 컴포넌트
    public Image colorIndicator;     // 마커 색상을 표시하는 Image 컴포넌트 (예: 배경색)
    public GameObject favoriteIcon;  // 즐겨찾기 아이콘 오브젝트 (Check 표시)

    public void Setup(MarkerData data)
    {
        _data = data;

        if (nameText != null)
        {
            nameText.text = data.Name;
        }

        // 즐겨찾기 (Check) 표시
        if (favoriteIcon != null)
        {
            // IsFavorite 값에 따라 아이콘을 활성화/비활성화
            favoriteIcon.SetActive(data.IsFavorite);
        }

        // 3. **핵심 수정: 색상 코드를 기반으로 이미지 교체**
        if (colorIndicator != null)
        {
            // MarkerColorImageManager의 Instance를 찾아 이미지 요청 (싱글톤 사용 가정)
            MarkerColorImageManager manager = MarkerColorImageManager.Instance;

            if (manager != null)
            {
                Sprite newSprite = manager.GetSpriteByColorCode(data.ColorCode);
                if (newSprite != null)
                {
                    colorIndicator.sprite = newSprite; // 이미지 교체
                    // 기존 색상 오버레이를 제거하고 이미지의 원래 색상을 표시합니다.
                    colorIndicator.color = Color.white;
                }
            }
            else
            {
                Debug.LogError("MarkerColorImageManager 인스턴스를 찾을 수 없습니다. 기본 색칠 로직으로 대체합니다.");

                // 관리자를 찾을 수 없을 때 임시로 Hex 색상을 적용하는 대체 로직
                if (ColorUtility.TryParseHtmlString(data.ColorCode, out Color newColor))
                {
                    colorIndicator.color = newColor;
                    colorIndicator.sprite = null;
                }
            }
        }

        Debug.Log($"[UI Data] 마커 UI에 데이터 저장 완료. ID: {data.Id}");
    }

    public void OnMarkerIconClicked()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
        {
            if (Data == null)
            {
                Debug.LogError("클릭된 마커 아이콘에 데이터가 없습니다.");
                return;
            }

            Debug.Log($"[Click Event] 마커 아이콘 클릭. ID: {Data.Id}, 이름: {Data.Name}");

            // 팝업 관리자에게 이 마커의 데이터를 전달하여 상세 팝업을 띄우도록 요청
            UIPopupManager popupManager = FindFirstObjectByType<UIPopupManager>();

            if (popupManager != null)
            {
                popupManager.ShowMarkerDetailPopup(Data);
            }
            else
            {
                Debug.LogError("UIPopupManager를 찾을 수 없습니다.");
            }

            lastClickTime = 0f;
        }
        else
        {
            // 싱글 클릭: 다음 더블 클릭을 위해 시간만 기록
            lastClickTime = Time.time;
            Debug.Log("[Click Event] 싱글 클릭: 다음 더블 클릭 대기 중...");
        }
    }
}