using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    [SerializeField] string nextScene = "sc_main";

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
            if (tapToSkip && (Input.anyKeyDown || Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Escape))) break;
            yield return null;
        }
        if (panelIntro) panelIntro.SetActive(false);
        if (panelLoading) panelLoading.SetActive(true);
        yield return StartCoroutine(LoadMainAsync());
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
        if (!loadingSlider && panelLoading)
        {
            loadingSlider = panelLoading.GetComponentInChildren<Slider>(true);
        }
        if (!percentText && panelLoading)
        {
            percentText = panelLoading.GetComponentInChildren<TextMeshProUGUI>(true);
        }
    }
#endif
}
