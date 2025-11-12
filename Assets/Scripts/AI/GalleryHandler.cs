using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GalleryHandler : MonoBehaviour
{
    [Header("UI References")]
    public RawImage displayImage;
    public Text feedbackText;

    private Texture2D loadedTexture;
    private Coroutine scaleRoutine;

    public void PickImage()
    {
        // 갤러리 열기
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path == null)
            {
                ShowFeedback("이미지 선택이 취소되었습니다.", Color.gray);
                return;
            }

            // 이미지 불러오기
            Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize: 1024);
            if (texture == null)
            {
                ShowFeedback("이미지를 불러올 수 없습니다.", Color.red);
                return;
            }

            loadedTexture = texture;
            displayImage.texture = loadedTexture;
            displayImage.color = Color.white;


            ShowFeedback("이미지가 성공적으로 로드되었습니다!", new Color(0.2f, 0.8f, 0.2f));
            

            // // 피드백 애니메이션 (살짝 확대 후 복귀)
            // if (scaleRoutine != null) StopCoroutine(scaleRoutine);
            // scaleRoutine = StartCoroutine(AnimateImageScale(displayImage.rectTransform));
        },
        "이미지를 선택하세요",
        "image/*");
    }

    // private IEnumerator AnimateImageScale(RectTransform rect)
    // {
    //     Vector3 startScale = Vector3.one;
    //     Vector3 targetScale = Vector3.one * 1.1f;
    //     float t = 0f;

    //     // 확대
    //     while (t < 1f)
    //     {
    //         rect.localScale = Vector3.Lerp(startScale, targetScale, t);
    //         t += Time.deltaTime * 3f;
    //         yield return null;
    //     }

    //     // 복귀
    //     t = 0f;
    //     while (t < 1f)
    //     {
    //         rect.localScale = Vector3.Lerp(targetScale, startScale, t);
    //         t += Time.deltaTime * 3f;
    //         yield return null;
    //     }
    //     rect.localScale = startScale;
    // }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            StartCoroutine(ClearFeedbackAfterDelay(3f));
        }
    }

    private IEnumerator ClearFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedbackText != null)
            feedbackText.text = "";
    }
}
