using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmPopup : MonoBehaviour
{
    public static ConfirmPopup Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;

    private Action onConfirm;

    private void Awake()
    {
        Instance = this;
        root.SetActive(false);

        btnConfirm.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            Close();
        });

        btnCancel.onClick.AddListener(Close);
    }

    public void Open(Action confirmAction)
    {
        // messageText.text = message;
        onConfirm = confirmAction;
        root.SetActive(true);
    }

    private void Close()
    {
        root.SetActive(false);
        onConfirm = null;
    }
}
