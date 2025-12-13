using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 선택된 MarkerInstance들을 회전시키는 표현 전용 애니메이터.
/// - 데이터 변경 ❌
/// - 회전 확정 판단 ❌
/// - 현재는 단일 선택 기준
/// - 다중 선택 확장 가능 구조
/// </summary>
public class MarkerRotateAnimator : MonoBehaviour
{
    public static MarkerRotateAnimator Instance { get; private set; }

    // =========================
    // Rotation Settings
    // =========================
    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 180f; // degrees per second
    [SerializeField] private bool loop = true;

    // =========================
    // Internal
    // =========================
    private readonly List<MarkerInstance> targets = new List<MarkerInstance>();
    private Coroutine rotateCo;

    // =========================
    // Unity Lifecycle
    // =========================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // =========================
    // Public API
    // =========================

    /// <summary>
    /// 회전 대상 설정 (단일 선택용)
    /// MarkerSelectionController에서 호출
    /// </summary>
    public void SetSingleTarget(MarkerInstance marker)
    {
        ClearTargets();

        if (marker != null)
            targets.Add(marker);

        RefreshRotationState();
    }

    /// <summary>
    /// 회전 대상 다중 설정 (확장용)
    /// 추후 다중 선택 컨트롤러에서 사용
    /// </summary>
    public void SetMultipleTargets(IEnumerable<MarkerInstance> markers)
    {
        ClearTargets();

        if (markers != null)
            targets.AddRange(markers);

        RefreshRotationState();
    }

    /// <summary>
    /// 모든 회전 중지
    /// </summary>
    public void StopRotate()
    {
        if (rotateCo != null)
        {
            StopCoroutine(rotateCo);
            rotateCo = null;
        }

        targets.Clear();
    }

    // =========================
    // Internal Control
    // =========================
    private void RefreshRotationState()
    {
        if (targets.Count == 0)
        {
            StopRotate();
            return;
        }

        if (rotateCo == null)
            rotateCo = StartCoroutine(RotateRoutine());
    }

    private void ClearTargets()
    {
        targets.Clear();
    }

    // =========================
    // Rotation Coroutine
    // =========================
    private IEnumerator RotateRoutine()
    {
        while (targets.Count > 0)
        {
            float delta = rotateSpeed * Time.deltaTime;

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                MarkerInstance marker = targets[i];

                // 파괴된 인스턴스 정리
                if (marker == null)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                marker.transform.Rotate(
                    Vector3.up,
                    delta,
                    Space.World
                );
            }

            if (!loop)
                break;

            yield return null;
        }

        rotateCo = null;
    }
}
