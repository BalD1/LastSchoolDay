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
    [SerializeField] private Image loadingBar;
    [SerializeField] private Image loadingText;
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;
    
    [SerializeField] private bool waitForInput = false;
    [SerializeField] private bool loadScene = false;

    private void Awake()
    {
        loadScene = false;
        waitForInput = false;
        pressAnyKeyText.gameObject.SetActive(false);
        loadingBar.fillAmount = 0;
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

            loadingBar.fillAmount = progressValue;

            if (asOp.progress >= .9f)
            {
                pressAnyKeyText.gameObject.SetActive(true);
                pressAnyKeyText.GetComponent<SimpleAnimatedTMP>().StartAnim();

                loadingText.gameObject.SetActive(false);
                waitForInput = true;

                if (loadScene) asOp.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
