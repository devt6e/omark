using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class DropdownMenu : MonoBehaviour
{
    [Header("References")]
    public Button toggleButton;         // 열고 닫는 버튼
    public RectTransform contentPanel;  // 펼쳐지는 패널
    public float expandedHeight = 250f; // 열렸을 때 높이
    public float animDuration = 0.25f;  // 애니메이션 시간

    private CanvasGroup canvasGroup;
    private bool isOpen = false;
    private float collapsedHeight = 0f;
    private Tween currentTween;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // 처음에는 접힌 상태
        contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, collapsedHeight);
        canvasGroup.alpha = 1f;

        toggleButton.onClick.AddListener(ToggleDropdown);
    }

    public void ToggleDropdown()
    {
        if (currentTween != null)
            currentTween.Kill();

        if (isOpen)
        {
            CloseDropdown();
        }
        else
        {
            OpenDropdown();
        }
    }

    private void OpenDropdown()
    {
        isOpen = true;

        Sequence seq = DOTween.Sequence();
        seq.Append(contentPanel.DOSizeDelta(
            new Vector2(contentPanel.sizeDelta.x, expandedHeight),
            animDuration
        ).SetEase(Ease.OutCubic));

        seq.Join(canvasGroup.DOFade(1f, animDuration));
        currentTween = seq;
    }

    private void CloseDropdown()
    {
        isOpen = false;

        Sequence seq = DOTween.Sequence();
        seq.Append(contentPanel.DOSizeDelta(
            new Vector2(contentPanel.sizeDelta.x, collapsedHeight),
            animDuration
        ).SetEase(Ease.InCubic));

        seq.Join(canvasGroup.DOFade(1f, animDuration));
        currentTween = seq;
    }
}
