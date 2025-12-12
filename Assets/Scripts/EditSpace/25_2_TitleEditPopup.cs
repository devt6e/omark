using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TitleEditPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;

    private Action<string> onConfirm;  
    private Action onCancel;

    private void Awake()
    {
        gameObject.SetActive(false);

        btnConfirm.onClick.AddListener(() =>
        {
            onConfirm?.Invoke(inputField.text);
            Close();
        });

        btnCancel.onClick.AddListener(() =>
        {
            onCancel?.Invoke();
            Close();
        });
    }

    public void Open(string currentTitle, Action<string> confirmCallback, Action cancelCallback = null)
    {
        inputField.text = currentTitle;
        onConfirm = confirmCallback;
        onCancel = cancelCallback;

        gameObject.SetActive(true);

        // 인풋 필드 자동 포커스
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
