using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

// IPointerDownHandler, IPointerUpHandler 인터페이스를 사용합니다.
public class MarkerInputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("3D 오브젝트 Layer")]
    public LayerMask markerLayer; // Inspector에서 'Marker' 레이어만 선택

    // 롱 프레스 시간 설정 (3초)
    private const float LONG_PRESS_TIME = 0.5f;
    private const float DRAG_TOLERANCE = 10f; // 10픽셀 이상 움직이면 드래그로 간주

    // 내부 상태 변수
    private bool isPressing = false;      // 현재 눌리고 있는 상태인지
    private bool isPanelShown = false;    // 패널이 이미 떴는지 확인
    private Coroutine longPressCoroutine; // 코루틴 참조
    private Vector2 pressPosition;        // 터치 시작 위치 (드래그와 구분하기 위함)
    private float pressTimer = 0f;       // 눌리고 있는 시간 카운트용

    // ======================================================================
    // 1. 입력 감지 및 롱 프레스 시작
    // ======================================================================

    // 터치 시작 시 호출 (IPointerDownHandler)
    public void OnPointerDown(PointerEventData eventData)
    {
        // 상태 설정
        isPressing = true;
        isPanelShown = false;
        pressPosition = eventData.position; // 시작 위치 저장
        pressTimer = 0f; // 타이머 초기화

        // 이전 코루틴이 실행 중이라면 중지
        if (longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
        }

        // 롱 프레스 체크 코루틴 시작 (시간만 잽니다)
        longPressCoroutine = StartCoroutine(CheckLongPressTimer());
    }

    // 터치 종료 시 호출 (IPointerUpHandler)
    public void OnPointerUp(PointerEventData eventData)
    {
        // 누르기 상태 해제
        isPressing = false;

        // 롱 프레스 코루틴 종료
        if (longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
        }

        // **[핵심 수정]**
        // 1. 드래그 여부 확인: 시작 위치(pressPosition)와 떼는 위치(eventData.position)를 비교
        if (Vector2.Distance(pressPosition, eventData.position) > DRAG_TOLERANCE)
        {
            Debug.Log("드래그 감지: 롱 프레스 취소됨.");
            return; // 드래그였으므로 여기서 종료
        }

        // 2. 시간 확인: 충분히 길게 눌렀는지 확인
        if (pressTimer >= LONG_PRESS_TIME)
        {
            // 롱 프레스 성공
            Debug.Log("[Click] 롱 프레스 성공.");
            ProcessLongPress(pressPosition); // 시작 위치를 사용
            isPanelShown = true;
        }
        else
        {
            // 짧게 눌렀다면 (일반 클릭에 해당)
            // 필요하다면 이곳에 일반 클릭 로직을 추가할 수 있습니다.
            Debug.Log("짧게 클릭됨.");
        }
    }

    // ======================================================================
    // 2. 롱 프레스 로직 (코루틴)
    // ======================================================================

    // **[수정]** 이제 이 코루틴은 시간을 재는 역할만 합니다.
    private IEnumerator CheckLongPressTimer()
    {
        while (isPressing)
        {
            pressTimer += Time.deltaTime;

            // 롱 프레스 시간이 경과하면, 패널이 뜰 준비를 합니다. (OnPointerUp에서 최종 처리)
            if (pressTimer >= LONG_PRESS_TIME && !isPanelShown)
            {
                // 패널이 뜬 것처럼 미리 플래그를 세워 두어 중복 처리를 방지할 수도 있지만,
                // 여기서는 단순히 시간이 다 되었음을 알리고 OnPointerUp에 처리를 맡깁니다.
                // Log는 주석 처리하여 깔끔하게 유지합니다.
                // Debug.Log("3초 경과."); 
            }
            yield return null;
        }
    }

    // ======================================================================
    // 3. 3D Raycasting 및 패널 호출
    // ======================================================================

    private void ProcessLongPress(Vector2 screenPosition)
    {
        // 1. Raycasting 수행
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        // 마커 레이어(LayerMask)에 대해서만 Raycast 발사
        if (Physics.Raycast(ray, out hit, 100f, markerLayer))
        {
            // 2. 충돌한 오브젝트에서 ARMarkerData를 찾습니다.
            ARMarkerData arData = hit.collider.GetComponent<ARMarkerData>();

            if (arData != null)
            {
                // 3. 3D 마커 클릭 성공: UIPopupManager 호출
                Show3DControlPanel(arData);
                return;
            }
        }

        Debug.LogWarning("롱 프레스가 감지되었으나, 해당 지점에 마커가 없습니다.");
    }

    private void Show3DControlPanel(ARMarkerData arData)
    {
        UIPopupManager popupManager = FindAnyObjectByType<UIPopupManager>();

        if (popupManager != null)
        {
            popupManager.Show3DControlPanel(arData);
        }
    }
}