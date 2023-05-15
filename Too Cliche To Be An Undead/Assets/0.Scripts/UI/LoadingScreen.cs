using BalDUtilities.Misc;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private RectTransform[] loadingIcons;
    [SerializeField] private GameObject[] imagesToHideOnLoadComplete;
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;
    
    [SerializeField] private bool waitForInput = false;
    [SerializeField] private bool loadScene = false;

    private void Awake()
    {
        loadScene = false;
        waitForInput = false;
        pressAnyKeyText.gameObject.SetActive(false);
    }

    private void Start()
    {
        foreach (RectTransform item in loadingIcons)
            LeanTween.rotate(item, 360, 2).setRepeat(-1);
    }

    private void Update()
    {
        if (Input.anyKey && waitForInput) loadScene = true;
    }

    public void StartLoad(ButtonArgs_Scene scene)
    {
        string sceneName = EnumsExtension.EnumToString(scene.GetArgs);

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asOp = SceneManager.LoadSceneAsync(sceneName);
        asOp.allowSceneActivation = false;

        while (!asOp.isDone)
        {
            float progressValue = Mathf.Clamp01(asOp.progress / .9f);

            if (asOp.progress >= .9f)
            {
                pressAnyKeyText.gameObject.SetActive(true);
                pressAnyKeyText.GetComponent<SimpleAnimatedTMP>().StartAnim();

                foreach (var item in imagesToHideOnLoadComplete) item.SetActive(false);

                waitForInput = true;

                if (loadScene) asOp.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
