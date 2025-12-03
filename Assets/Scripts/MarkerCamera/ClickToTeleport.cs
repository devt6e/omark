using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToTeleport : MonoBehaviour, IPointerClickHandler
{
    // [Inspector 연결]
    [Header("카메라 이동 설정")]
    public Transform cameraPivot; // XR Origin의 Camera Offset Transform을 연결
    public LayerMask placementLayer; // 충돌할 바닥/물체 레이어 (MarkerPlacer와 동일)
    public float clickOffset = 1.7f; // 카메라를 바닥 위로 띄울 높이 (일반적인 사람 눈높이)

    // 내부 상태 변수 (더블 클릭 감지용)
    private float lastClickTime = 0f;
    private const float DOUBLE_CLICK_TIME = 0.3f; // 더블 클릭 최대 시간 간격 (0.3초)

    // 마우스/터치 입력 시 호출
    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
        {
            // **더블 클릭 판정**
            TeleportToClickedPoint(eventData.position);
            lastClickTime = 0f;
        }
        else
        {
            // 싱글 클릭: 다음 더블 클릭을 위해 시간만 기록
            lastClickTime = Time.time;
        }
    }

    private void TeleportToClickedPoint(Vector2 screenPosition)
    {
        if (cameraPivot == null)
        {
            Debug.LogError("카메라 Pivot이 연결되지 않았습니다.");
            return;
        }

        // 1. Raycast 수행 (터치 위치에서 3D 공간으로 광선 발사)
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, placementLayer))
        {
            // 2. Teleport Target Calculation
            // XZ 평면 위치는 충돌 지점(hit.point)을 사용하고, Y축은 고정된 clickOffset 높이를 사용합니다.
            Vector3 targetPosition = new Vector3(hit.point.x, hit.point.y + clickOffset, hit.point.z);

            // 3. Apply Teleportation (순간 이동)
            cameraPivot.position = targetPosition;

            Debug.Log($"[Teleport] 카메라를 {targetPosition}으로 순간 이동했습니다.");
        }
        else
        {
            Debug.LogWarning("텔레포트 실패: 바닥 또는 물체 표면을 찾을 수 없습니다.");
        }
    }
}