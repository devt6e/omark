using System;
using UnityEngine;

/// <summary>
/// 마커 즐겨찾기 / 검색 필터 상태 관리자
/// - 데이터 직접 접근 ❌
/// - UI 직접 제어 ❌
/// - 상태 변경 시 Apply 요청만 수행
/// </summary>
public class MarkerFilterController : MonoBehaviour
{
    public static MarkerFilterController Instance { get; private set; }

    // =========================
    // Filter State
    // =========================

    public bool FavoriteOnly { get; private set; } = false;
    public string SearchKeyword { get; private set; } = string.Empty;

    // =========================
    // Events
    // =========================

    /// <summary>
    /// 필터 상태가 변경되었음을 알림
    /// </summary>
    public event Action OnFilterChanged;

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
    // Favorite Filter
    // =========================

    public void ToggleFavorite()
    {
        FavoriteOnly = !FavoriteOnly;
        NotifyChanged();
    }

    public void SetFavorite(bool value)
    {
        if (FavoriteOnly == value)
            return;

        FavoriteOnly = value;
        NotifyChanged();
    }

    // =========================
    // Search Filter
    // =========================

    public void SetSearchKeyword(string keyword)
    {
        keyword = keyword?.Trim() ?? string.Empty;

        if (SearchKeyword == keyword)
            return;

        SearchKeyword = keyword;
        NotifyChanged();
    }

    public void ClearSearch()
    {
        if (string.IsNullOrEmpty(SearchKeyword))
            return;

        SearchKeyword = string.Empty;
        NotifyChanged();
    }

    // =========================
    // Utility
    // =========================

    private void NotifyChanged()
    {
        OnFilterChanged?.Invoke();
    }
}
