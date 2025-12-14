using UnityEngine;

/// <summary>
/// 즐겨찾기 보기 모드 전용 컨트롤러
/// </summary>
public class FavoriteViewController : MonoBehaviour
{
    [SerializeField] private MarkerSlotSpawner slotSpawner;
    [SerializeField] private MarkerRotateAnimator rotateAnimator;

    public bool IsFavoriteOnly { get; private set; }

    public void ToggleFavoriteView()
    {
        IsFavoriteOnly = !IsFavoriteOnly;
        Apply();
    }

    public void SetFavoriteView(bool value)
    {
        IsFavoriteOnly = value;
        Apply();
    }

    private void Apply()
    {
        // 슬롯 필터
        slotSpawner.ApplyFavoriteFilter(IsFavoriteOnly);

        // 인스턴스 회전 강조
        if (IsFavoriteOnly)
        {
            rotateAnimator.RotateFavorites();
        }
        else
        {
            rotateAnimator.StopRotate();
        }
    }
}
