using BalDUtilities.VectorUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TextPopup : MonoBehaviour
{
    private float maxLifetime;

    private SCRPT_TextPopupComponents.HitComponents componentsNeeded;

    private TextMeshPro textMesh;

    private Color textColor;

    private const float secondLifetimePart = .5f;

    public static Queue<GameObject> popupPool = new Queue<GameObject>();

    public static TextPopup Create(string text, Vector2 pos)
    {
        GameObject txtPopupGo;
        if (popupPool.Count > 0) txtPopupGo = popupPool.Dequeue();
        else txtPopupGo = Instantiate(GameAssets.Instance.TextPopupPF, pos, Quaternion.identity);

        TextPopup txtPopup = txtPopupGo.GetComponent<TextPopup>();

        txtPopup.Setup(text);

        txtPopupGo.SetActive(true);

        return txtPopup;
    }
    public static TextPopup Create(string text, Transform parent)
    {
        GameObject txtPopupGo = Instantiate(GameAssets.Instance.TextPopupPF, parent);

        TextPopup txtPopup = txtPopupGo.GetComponent<TextPopup>();

        txtPopup.Setup(text);

        return txtPopup;
    }
    public static TextPopup Create(string text, Vector2 pos, SCRPT_TextPopupComponents.HitComponents components)
    {
        GameObject txtPopupGo = Instantiate(GameAssets.Instance.TextPopupPF, pos, Quaternion.identity);

        TextPopup txtPopup = txtPopupGo.GetComponent<TextPopup>();

        txtPopup.Setup(text, components);

        return txtPopup;
    }
    public static TextPopup Create(string text, Transform parent, SCRPT_TextPopupComponents.HitComponents components)
    {
        GameObject txtPopupGo = Instantiate(GameAssets.Instance.TextPopupPF, parent);

        TextPopup txtPopup = txtPopupGo.GetComponent<TextPopup>();

        txtPopup.Setup(text, components);

        return txtPopup;
    }

    public void Setup(string text)
    {
        Setup(text, GameAssets.BaseComponents);
    }
    public void Setup(string text, SCRPT_TextPopupComponents.HitComponents components)
    {
        componentsNeeded = components;

        textMesh.SetText(text);

        textMesh.fontSize = componentsNeeded.fontSize;
        textMesh.color = componentsNeeded.color;

        maxLifetime = componentsNeeded.lifetime;

        // 1/2 chances of inversing x movements
        if (Random.Range(0, 2) == 0)
            componentsNeeded.speedMovements.x *= -1;
    }

    private void Awake()
    {
        textMesh = this.transform.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        Movements();

        Effects();

        componentsNeeded.lifetime -= Time.deltaTime;

        if (componentsNeeded.lifetime <= 0)
            Disappear();
    }

    private void Movements()
    {
        if (componentsNeeded.lifetime < maxLifetime * secondLifetimePart)
        {
            //Second part of lifetime
            this.transform.position += componentsNeeded.speedMovements * Time.deltaTime;

            if (componentsNeeded.speedMovements.x < 0)
                componentsNeeded.speedMovements = VectorClamps.ClampVector3(componentsNeeded.speedMovements - componentsNeeded.speedMovements * Time.deltaTime, new Vector3(float.MinValue, float.MinValue), new Vector3(0, float.MaxValue));
            else if (componentsNeeded.speedMovements.x > 0)
                componentsNeeded.speedMovements = VectorClamps.ClampVector3(componentsNeeded.speedMovements - componentsNeeded.speedMovements * Time.deltaTime, 0, float.MaxValue);
        }
    }

    private void Effects()
    {
        if (componentsNeeded.lifetime > maxLifetime * secondLifetimePart)
        {
            //First part of lifetime
            this.transform.localScale += Vector3.one * componentsNeeded.increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            //Second part of lifetime
            Vector3 scale = this.transform.localScale - Vector3.one * componentsNeeded.decreaseScaleAmount * Time.deltaTime;

            if (scale.x < 0)
                scale.x = 0;
            if (scale.y < 0)
                scale.y = 0;

            this.transform.localScale = scale;
        }
    }

    private void Disappear()
    {
        textColor.a -= componentsNeeded.disapearSpeed * Time.deltaTime;
        textMesh.color = textColor;
        if (textColor.a <= 0)
        {
            this.gameObject.SetActive(false);
            popupPool.Enqueue(this.gameObject);
        }
    }
}
