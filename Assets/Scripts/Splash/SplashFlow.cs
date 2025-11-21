using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SplashFlow : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject panelIntro;
    [SerializeField] GameObject panelLoading;

    [Header("Loading UI (choose one)")]
    [SerializeField] Slider loadingSlider;
    [SerializeField] TextMeshProUGUI percentText;

    [Header("Flow Options")]
    [SerializeField] float introHold = 1.2f;
    [SerializeField] bool tapToSkip = true;

    [Header("Next Scene")]
    [SerializeField] string nextScene = "sc_login";

    void Start()
    {
        if (panelIntro) panelIntro.SetActive(true);
        if (panelLoading) panelLoading.SetActive(false);
        if (loadingSlider) loadingSlider.value = 0f;
        if (percentText) percentText.text = "0%";
        StartCoroutine(Flow());
    }

    IEnumerator Flow()
    {
        float t = 0f;
        while (t < introHold)
        {
            t += Time.deltaTime;
            if (tapToSkip && IsSkipPressed()) break;
            yield return null;
        }
        if (panelIntro) panelIntro.SetActive(false);
        if (panelLoading) panelLoading.SetActive(true);
        yield return LoadMainAsync();
    }

    bool IsSkipPressed()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(0)) return true;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) return true;
        if (Input.anyKeyDown || Input.GetKeyDown(KeyCode.Escape)) return true;
#endif
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        if (Keyboard.current != null && (Keyboard.current.anyKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)) return true;
#endif
        return false;
    }

    IEnumerator LoadMainAsync()
    {
        yield return new WaitForSeconds(0.2f);
        var op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float displayed = 0f;
        while (!op.isDone)
        {
            float target = Mathf.Clamp01(op.progress / 0.9f);
            displayed = Mathf.MoveTowards(displayed, target, Time.deltaTime * 1.5f);
            UpdateProgress(displayed);
            if (displayed >= 0.999f)
            {
                UpdateProgress(1f);
                yield return new WaitForSeconds(0.1f);
                op.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    void UpdateProgress(float v)
    {
        if (loadingSlider) loadingSlider.value = v;
        if (percentText) percentText.text = Mathf.RoundToInt(v * 100f).ToString() + "%";
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!panelIntro)
        {
            var go = GameObject.Find("PanelIntro");
            if (go) panelIntro = go;
        }
        if (!panelLoading)
        {
            var go = GameObject.Find("PanelLoading");
            if (go) panelLoading = go;
        }
        if (!loadingSlider && panelLoading) loadingSlider = panelLoading.GetComponentInChildren<Slider>(true);
        if (!percentText && panelLoading) percentText = panelLoading.GetComponentInChildren<TextMeshProUGUI>(true);
    }
#endif
}
