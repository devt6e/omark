using UnityEngine;

public class GalleryOpener : MonoBehaviour
{
    public void OpenGallery()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent", intentClass.GetStatic<string>("ACTION_PICK"));

                AndroidJavaClass uriClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media");
                AndroidJavaObject uriObject = uriClass.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI");

                intentObject.Call<AndroidJavaObject>("setData", uriObject);
                intentObject.Call<AndroidJavaObject>("setType", "image/*");

                currentActivity.Call("startActivity", intentObject);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("갤러리 열기 실패: " + e.Message);
        }
#else
        Debug.Log("에디터에서는 갤러리 기능이 비활성화되어 있습니다.");
#endif
    }
}