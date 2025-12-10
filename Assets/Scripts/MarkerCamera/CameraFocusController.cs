using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    // [Inspector ����]
    [Header("ī�޶� �̵� ���")]
    // XR Origin ������ Camera Offset Transform�� �����մϴ�.
    public Transform cameraPivot;

    [Header("��Ŀ�� ����")]
    public float focusDistance = 10.0f; // ��Ŀ�κ��� ������ �Ÿ� (����)
    public float moveSpeed = 5.0f;     // �̵� �ӵ� (�ε巯�� �̵��� ���� �ӵ�)
    public float rotationSpeed = 10.0f; // ȸ�� �ӵ�

    // ��Ŀ�� �� ���̵��� ī�޶� �̵� �� ȸ����Ű�� �Լ�
    public void FocusOnMarker(Transform markerTransform)
    {
        if (cameraPivot == null || markerTransform == null)
        {
            Debug.LogError("ī�޶� Pivot �Ǵ� ��Ŀ Transform�� ������� �ʾҽ��ϴ�.");
            return;
        }

        Vector3 markerCenter = markerTransform.position;

        // 1. ī�޶� ��ǥ ��ġ ���
        // ��Ŀ ��ġ���� ���� �Ÿ���ŭ '�ڷ�' ������ ������ ��ǥ�� �մϴ�.
        Vector3 targetPosition = markerCenter - (markerCenter - cameraPivot.position).normalized * focusDistance;

        // **ī�޶� Y�� ���� ���� (���� ��û)**
        float fixedY = cameraPivot.position.y;
        targetPosition.y = fixedY;

        // 2. ī�޶� ��ǥ ȸ�� ��� (��Ŀ�� �ٶ󺸰�)
        Quaternion cameraTargetRotation = Quaternion.LookRotation(markerCenter - targetPosition, Vector3.up);

        // 3. **[�ٽ� ����] ��Ŀ�� ȸ�� ��� �� ���� (ī�޶� ����)**
        Transform mainCameraTransform = Camera.main.transform;

        // ��Ŀ���� ī�޶��� ���� ��ġ�� ���ϴ� ���� ���� ���
        Vector3 markerLookDirection = mainCameraTransform.position - markerCenter;
        markerLookDirection.y = 0; // Y�� ���� (���� ȸ����)

        Quaternion markerTargetRotation = Quaternion.identity;

        if (markerLookDirection != Vector3.zero)
        {
            // LookRotation�� ����� ȸ�� �� ����
            markerTargetRotation = Quaternion.LookRotation(markerLookDirection);

            // ��Ŀ ��ġ �� ����ߴ� 180�� ���� �ڵ�
            markerTargetRotation *= Quaternion.Euler(0, 180, 0);
        }

        // **��Ŀ�� ȸ�� ��� ����**
        markerTransform.rotation = markerTargetRotation;

        // 4. �ε巯�� ī�޶� �̵� �� ȸ�� �ڷ�ƾ ����
        StartCoroutine(MoveAndRotate(targetPosition, cameraTargetRotation));

        Debug.Log($"[Focus] ī�޶� ��Ŀ {markerTransform.name}���� �̵� ��û�߽��ϴ�.");
    }

    private System.Collections.IEnumerator MoveAndRotate(Vector3 pos, Quaternion rot)
    {
        while (Vector3.Distance(cameraPivot.position, pos) > 0.01f || Quaternion.Angle(cameraPivot.rotation, rot) > 0.1f)
        {
            // ��ġ Lerp (�ε巯�� �̵�)
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, pos, Time.deltaTime * moveSpeed);

            // ȸ�� Lerp (�ε巯�� ȸ��)
            cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, rot, Time.deltaTime * rotationSpeed);

            yield return null;
        }
        // ���� ��ġ/ȸ�� Ȯ��
        cameraPivot.position = pos;
        cameraPivot.rotation = rot;
    }
}