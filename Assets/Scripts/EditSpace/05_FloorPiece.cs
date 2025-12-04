using UnityEngine;

public class FloorPiece : MonoBehaviour
{
    private GameObject highlightObj;

    [Header("Highlight Prefab (Quad)")]
    public GameObject highlightPrefab;

    // 바닥 영역 정보
    public Bounds GetBounds()
    {
        return new Bounds(transform.position, transform.localScale);
    }

    // ================================
    // 선택
    // ================================
    public void Select()
    {
        if (highlightObj != null) return;

        highlightObj = Instantiate(highlightPrefab, transform);
        highlightObj.transform.localPosition = Vector3.zero;
        highlightObj.transform.localRotation = Quaternion.identity;

        // 바닥과 동일한 스케일 적용
        highlightObj.transform.localScale = Vector3.one;

        // 하이라이트는 바닥보다 조금 위에 배치해 z-fighting 방지
        highlightObj.transform.localPosition = new Vector3(0, 0.51f, 0);

        // 회전 적용 (Quad를 바닥 위로 눕히기)
        highlightObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
    }

    // ================================
    // 선택 해제
    // ================================
    public void Deselect()
    {
        if (highlightObj != null)
        {
            Destroy(highlightObj);
            highlightObj = null;
        }
    }
}
