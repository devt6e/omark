using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordEyeToggle : MonoBehaviour
{
    public TMP_InputField input;
    public Image img;
    public Sprite eyeOpen;
    public Sprite eyeClosed;

    bool shown;

    void OnEnable()
    {
        SyncIcon();
    }

    public void Toggle()
    {
        shown = !shown;
        input.contentType = shown ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        input.ForceLabelUpdate();
        SyncIcon();
    }

    void SyncIcon()
    {
        if (img) img.sprite = shown ? eyeOpen : eyeClosed;
    }
}
