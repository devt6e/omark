using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider))]
public class FurniturePiece : MonoBehaviour
{
    [Header("Runtime Info")]
    [SerializeField] private string id;
    [SerializeField] private Vector3 size;   // 가로(x), 높이(y), 세로(z)

    [Header("References")]
    [SerializeField] private Transform pivot; // 기즈모 기준점 (기본 transform)

    [Header("Visual")]
    public GameObject highlightObj; // 선택 시 표시되는 하이라이트(선택 Outline 등)

    public string Id => id;
    public Vector3 Size => size;
    public Transform Pivot => pivot != null ? pivot : transform;

    private bool isSelected = false;

    private void Awake()
    {
        // Guid 자동 생성
        if (string.IsNullOrEmpty(id))
            id = Guid.NewGuid().ToString();

        // pivot이 비어있다면 자신의 Transform 사용
        if (pivot == null)
            pivot = transform;

        // 하이라이트 비활성화
        if (highlightObj != null)
            highlightObj.SetActive(false);
    }

    /// <summary>
    /// 가구 생성 직후 초기 사이즈를 적용할 때 사용.
    /// </summary>
    public void Initialize(Vector3 newSize)
    {
        size = newSize;
        ApplySize(size);
    }

    /// <summary>
    /// 실제 Mesh/Collider의 크기를 변경한다.
    /// </summary>
    public void ApplySize(Vector3 newSize)
    {
        size = newSize;
        transform.localScale = size;

        UpdateCollider();
    }

    /// <summary>
    /// 선택 표시용 하이라이트
    /// </summary>
    public void Select()
    {
        isSelected = true;
        if (highlightObj != null)
            highlightObj.SetActive(true);
    }

    public void Deselect()
    {
        isSelected = false;
        if (highlightObj != null)
            highlightObj.SetActive(false);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    /// <summary>
    /// 이동 / 충돌 / 스냅 계산에 필요한 Bounds 반환
    /// </summary>
    public Bounds GetBounds()
    {
        var col = GetComponent<BoxCollider>();
        return col.bounds;
    }

    /// <summary>
    /// 사이즈 변경 시 Collider도 자동 반영
    /// </summary>
    private void UpdateCollider()
    {
        var col = GetComponent<BoxCollider>();
        if (col == null) return;

        col.size = Vector3.one;
        col.center = Vector3.zero;
    }
}
