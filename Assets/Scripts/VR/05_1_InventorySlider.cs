using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class InventorySlider : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // [Inspector 설정]
    [Header("슬라이드 패널 설정")]
    public RectTransform panelRect; // 슬라이드될 마커 리스트 패널의 RectTransform
    public float transitionSpeed = 15f; // 이동 속도
    public float dragThreshold = 0.05f; // 드래그로 인정하는 최소 비율 (화면 높이 대비 5%)

    [Header("Pivot 위치 설정")]
    public float hiddenPivotY = 0f;    // 숨김 상태일 때 Pivot Y (기본값 1.0)
    public float visiblePivotY = 0f;  // 표시 상태일 때 Pivot Y (기본값 1.07)

    [Header("refs")]
    [SerializeField] private CameraController3D cameraController;
    // 내부 상태 변수
    private Vector2 targetPosition;
    private float hiddenY;     // 숨김 상태의 Y 위치 (anchoredPosition)
    private float visibleY;    // 표시 상태의 Y 위치 (anchoredPosition)

    void Start()
    {
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
            if (panelRect == null)
            {
                Debug.LogError("패널 RectTransform이 필요합니다!");
                enabled = false;
                return;
            }
        }

        // 1. Y 위치 계산
        // visibleY: 화면 하단에 딱 붙은 상태 (Y = 0)
        visibleY = 0f;

        // hiddenY: 패널 높이만큼 아래로 내려간 상태
        hiddenY = -panelRect.rect.height * 0.5f;

        // 2. 초기 위치 설정 (숨김 상태)
        targetPosition = new Vector2(panelRect.anchoredPosition.x, hiddenY);
        panelRect.anchoredPosition = targetPosition;

        // 다시: 초기 Pivot Y를 숨김 상태 값으로 설정
        Vector2 currentPivot = panelRect.pivot;
        panelRect.pivot = new Vector2(currentPivot.x, hiddenPivotY);
    }

    void Update()
    {
        // 3. 목표 위치로 부드럽게 이동
        panelRect.anchoredPosition = Vector2.Lerp(
            panelRect.anchoredPosition,
            targetPosition,
            Time.deltaTime * transitionSpeed
        );
    }

    // 드래그 중 호출 (IDragHandler)
    public void OnDrag(PointerEventData eventData)
    {
        cameraController.IsBlocked = true;
        // Y축 드래그에 따라 패널 위치 이동
        float newY = panelRect.anchoredPosition.y + eventData.delta.y;

        // 이동 범위 제한: 숨김(hiddenY) ~ 표시(visibleY)
        newY = Mathf.Clamp(newY, hiddenY, visibleY);

        panelRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, newY);

        // 드래그 중에는 즉시 목표 위치를 현재 위치로 설정
        targetPosition = panelRect.anchoredPosition;
    }

    // 드래그 종료 시 호출 (IEndDragHandler)
    public void OnEndDrag(PointerEventData eventData)
    {
        cameraController.IsBlocked = false;
        float screenHeight = Screen.height;
        float dragDistance = eventData.position.y - eventData.pressPosition.y; // 전체 드래그 이동 거리
        float dragRatio = Mathf.Abs(dragDistance) / screenHeight;

        float targetPivotY; // 최종 적용할 Pivot Y 값

        // 1. 임계값(threshold)을 넘었는지 확인
        if (dragRatio >= dragThreshold)
        {
            // 2. 드래그 방향에 따라 목표 위치 결정
            if (dragDistance > 0) // 위로 드래그 (패널 열기)
            {
                targetPosition = new Vector2(panelRect.anchoredPosition.x, visibleY);
                targetPivotY = visiblePivotY; // 1.07 적용
            }
            else // 아래로 드래그 (패널 닫기)
            {
                targetPosition = new Vector2(panelRect.anchoredPosition.x, hiddenY);
                targetPivotY = hiddenPivotY; // 1.0 적용
            }
        }
        else // 임계값을 넘지 못한 경우 (짧은 드래그)
        {
            // 현재 위치 기준으로 가까운 상태로 스냅
            if (panelRect.anchoredPosition.y > hiddenY + (visibleY - hiddenY) / 2f)
            {
                targetPosition = new Vector2(panelRect.anchoredPosition.x, visibleY);
                targetPivotY = visiblePivotY; // 1.07 적용
            }
            else
            {
                targetPosition = new Vector2(panelRect.anchoredPosition.x, hiddenY);
                targetPivotY = hiddenPivotY; // 1.0 적용
            }
        }

        // 다시: Pivot Y 값을 최종 상태에 맞게 변경
        Vector2 currentPivot = panelRect.pivot;
        panelRect.pivot = new Vector2(currentPivot.x, targetPivotY);
    }
}
