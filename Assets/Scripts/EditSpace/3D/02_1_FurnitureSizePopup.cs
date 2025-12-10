using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class FurnitureSizePopup : MonoBehaviour
{
    [Header("Inputs (cm 단위 입력 권장)")]
    public TMP_InputField nameInput; 
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    public TMP_InputField depthInput;

    public Button applyButton;
    public Button cancelButton;

    private Action<Vector3> onConfirm;

    private void Awake()
    {
        gameObject.SetActive(false);

        applyButton.onClick.AddListener(OnClickApply);
        cancelButton.onClick.AddListener(Hide);
    }

    public void Show(Action<Vector3> confirmCallback)
    {
        onConfirm = confirmCallback;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnClickApply()
    {
        if (!float.TryParse(widthInput.text, out float w)) w = 100;
        if (!float.TryParse(heightInput.text, out float h)) h = 100;
        if (!float.TryParse(depthInput.text, out float d)) d = 100;

        // cm → m 변환 (FloorPiece와 UI 규칙 동일 유지)
        Vector3 size = new Vector3(w, h, d) * 0.01f;

        onConfirm?.Invoke(size);
        Hide();
    }
}
