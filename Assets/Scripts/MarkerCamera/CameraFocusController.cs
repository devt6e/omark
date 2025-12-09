using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    // [Inspector 연결]
    [Header("카메라 이동 대상")]
    // XR Origin 하위의 Camera Offset Transform을 연결합니다.
    public Transform cameraPivot;

    [Header("포커스 설정")]
    public float focusDistance = 10.0f; // 마커로부터 떨어질 거리 (미터)
    public float moveSpeed = 5.0f;     // 이동 속도 (부드러운 이동을 위한 속도)
    public float rotationSpeed = 10.0f; // 회전 속도

    // 마커가 잘 보이도록 카메라를 이동 및 회전시키는 함수
    public void FocusOnMarker(Transform markerTransform)
    {
        if (cameraPivot == null || markerTransform == null)
        {
            Debug.LogError("카메라 Pivot 또는 마커 Transform이 연결되지 않았습니다.");
            return;
        }

        // 1. **목표 위치 계산**: 마커 위치에서 일정 거리만큼 '뒤로' 물러선 지점
        // 현재 카메라 회전 축이 아닌, 마커의 위치를 기준으로 World Space에서 계산합니다.
        Vector3 markerCenter = markerTransform.position;

        // 카메라가 마커를 바라보게 하기 위해, 마커 위치에서 FocusDistance만큼 '뒤로' 물러납니다.
        // 여기서는 World Up을 기준으로 마커 뒤쪽으로 1.5m 이동합니다.
        Vector3 targetPosition = markerCenter - (markerCenter - cameraPivot.position).normalized * focusDistance;

        // y축 고정
        float fixedY = cameraPivot.position.y;
        targetPosition.y = fixedY;

        // 2. **카메라 회전 계산 (LookAt):**
        // 카메라 피벗이 마커의 중심을 바라보도록 회전량을 계산합니다.
        Quaternion targetRotation = Quaternion.LookRotation(markerCenter - targetPosition, Vector3.up);

        // 3. 부드러운 이동 및 회전 코루틴 시작
        StartCoroutine(MoveAndRotate(targetPosition, targetRotation));

        Debug.Log($"[Focus] 카메라를 마커 {markerTransform.name}으로 이동 요청했습니다.");
    }

    private System.Collections.IEnumerator MoveAndRotate(Vector3 pos, Quaternion rot)
    {
        while (Vector3.Distance(cameraPivot.position, pos) > 0.01f || Quaternion.Angle(cameraPivot.rotation, rot) > 0.1f)
        {
            // 위치 Lerp (부드러운 이동)
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, pos, Time.deltaTime * moveSpeed);

            // 회전 Lerp (부드러운 회전)
            cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, rot, Time.deltaTime * rotationSpeed);

            yield return null;
        }
        // 최종 위치/회전 확정
        cameraPivot.position = pos;
        cameraPivot.rotation = rot;
    }
}