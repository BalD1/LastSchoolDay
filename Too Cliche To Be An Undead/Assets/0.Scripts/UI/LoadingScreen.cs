using BalDUtilities.Misc;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private RectTransform[] loadingIcons;
    [SerializeField] private CanvasGroup imagesToHideOnLoadComplete;
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;
    
    private bool waitForInput = false;
    private bool loadScene = false;

    private bool onCompleteFlag = false;

    private void Awake()
    {
        loadScene = false;
        waitForInput = false;
    }

    private void Start()
    {
        foreach (RectTransform item in loadingIcons)
            LeanTween.rotate(item, 360, 2).setRepeat(-1).setIgnoreTimeScale(true);

        LeanTween.delayedCall(Random.Range(.8f, 1.2f), () =>
        {
            StartCoroutine(LoadSceneAsync(LoadingScreenManager.SceneToLoad));
        }).setIgnoreTimeScale(true);
        
    }

    private void Update()
    {
        if (Input.anyKey && waitForInput) loadScene = true;
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asOp = SceneManager.LoadSceneAsync(sceneName);
        asOp.allowSceneActivation = false;

        while (!asOp.isDone)
        {
            float progressValue = Mathf.Clamp01(asOp.progress / .9f);
            if (progressValue >= .9f)
            {
                SetOnCompleteState();

                if (loadScene) asOp.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void SetOnCompleteState()
    {
        if (onCompleteFlag) return;
        onCompleteFlag = true;

        LeanTween.value(0, 1, .5f).setOnUpdate((float val) =>
        {
            Color c = pressAnyKeyText.color;
            c.a = val;
            pressAnyKeyText.color = c;
        }).setIgnoreTimeScale(true);

        //pressAnyKeyText.GetComponent<SimpleAnimatedTMP>().StartAnim();

        imagesToHideOnLoadComplete.LeanAlpha(0, .5f).setIgnoreTimeScale(true);

        waitForInput = true;
    }
}
