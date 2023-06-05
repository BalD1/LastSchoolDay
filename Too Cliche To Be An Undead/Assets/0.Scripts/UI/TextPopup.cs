using BalDUtilities.VectorUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TextPopup : MonoBehaviour
{
    [SerializeField] [ReadOnly] private float maxLifetime;
    [SerializeField][ReadOnly] private float currentLifetime;

    [SerializeField] [ReadOnly] private Vector3 speed;

    [SerializeField] [ReadOnly] private SCRPT_TextPopupComponents.HitComponents componentsNeeded;

    private TextMeshPro textMesh;

    private Color textColor;

    private const float secondLifetimePart = .5f;
    private static int sortingOrder;

    public static Queue<GameObject> popupPool = new Queue<GameObject>();

    #region Create

    /// <summary>
    /// Create or pools a new "<b><see cref="TextPopup"/></b>" displaying "<b><paramref name="text"/></b>" at "<b><paramref name="pos"/></b>" position.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static TextPopup Create(string text, Vector2 pos) => Create(text, pos, GameAssets.BaseComponents);
    /// <summary>
    /// Create or pools a new "<b><see cref="TextPopup"/></b>" displaying "<b><paramref name="text"/></b>".
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static TextPopup Create(string text, Transform parent) => Create(text, parent, GameAssets.BaseComponents);
    /// <summary>
    /// Create or pools a new "<b><see cref="TextPopup"/></b>" displaying "<b><paramref name="text"/></b>" at "<b><paramref name="pos"/></b>" position and with "<b><paramref name="components"/></b>" style.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pos"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    public static TextPopup Create(string text, Vector2 pos, SCRPT_TextPopupComponents.HitComponents components)
    {
        GameObject txtPopupGo = GetFromPoolOrInstantiate(pos);

        TextPopup txtPopup = txtPopupGo.GetComponent<TextPopup>();

        txtPopup.Setup(text, components);

        return txtPopup;
    }
    /// <summary>
    /// Create or pools a new "<b><see cref="TextPopup"/></b>" displaying "<b><paramref name="text"/></b>" and with "<b><paramref name="components"/></b>" style.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static TextPopup Create(string text, Transform parent, SCRPT_TextPopupComponents.HitComponents components)
    {
        GameObject txtPopupGo = GetFromPoolOrInstantiate(parent);

        TextPopup txtPopup = txtPopupGo.GetComponent<TextPopup>();

        txtPopup.Setup(text, components);

        return txtPopup;
    }

    private static GameObject GetFromPoolOrInstantiate(Vector2 pos)
    {
        GameObject txtPopupGo;
        if (popupPool.Count > 0)
        {
            txtPopupGo = popupPool.Dequeue();

            if (txtPopupGo == null)
            {
                txtPopupGo = Instantiate(GameAssets.Instance.TextPopupPF, pos, Quaternion.identity);
            }

            txtPopupGo.transform.SetParent(GameManager.Instance.InstantiatedMiscParent);
            txtPopupGo.transform.position = pos;
            txtPopupGo.transform.localScale = Vector3.one;
            txtPopupGo.SetActive(true);
        }
        else txtPopupGo = Instantiate(GameAssets.Instance.TextPopupPF, pos, Quaternion.identity);

        return txtPopupGo;
    }
    private static GameObject GetFromPoolOrInstantiate(Transform parent)
    {
        GameObject txtPopupGo;
        if (popupPool.Count > 0)
        {
            txtPopupGo = popupPool.Dequeue();

            if(txtPopupGo == null)
            {
                txtPopupGo = Instantiate(GameAssets.Instance.TextPopupPF, parent);
            }

            if (parent != null)
                txtPopupGo.transform.SetParent(parent, false);

            txtPopupGo.transform.localPosition = Vector3.zero;
            txtPopupGo.transform.localScale = Vector3.one;
            txtPopupGo.SetActive(true);
        }
        else txtPopupGo = Instantiate(GameAssets.Instance.TextPopupPF, parent);

        return txtPopupGo;
    } 

    #endregion

    public void Setup(string text) => Setup(text, GameAssets.BaseComponents);
    public void Setup(string text, SCRPT_TextPopupComponents.HitComponents components)
    {
        componentsNeeded = components;

        textMesh.SetText(text);

        textMesh.fontSize = componentsNeeded.fontSize;
        textMesh.color = textColor = componentsNeeded.color;

        maxLifetime = currentLifetime = componentsNeeded.lifetime;

        speed = componentsNeeded.speedMovements;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        // 1/2 chances of inversing x movements
        if (Random.Range(0, 2) == 0)
            speed.x *= -1;
    }

    private void Awake() => textMesh = this.transform.GetComponent<TextMeshPro>();

    private void Update()
    {
        Movements();

        Effects();

        currentLifetime -= Time.deltaTime;

        if (currentLifetime <= 0)
            Disappear();
    }

    private void Movements()
    {
        if (currentLifetime < maxLifetime * secondLifetimePart)
        {
            //Second part of lifetime
            this.transform.position += speed * Time.deltaTime;

            if (speed.x < 0)
                speed = VectorClamps.ClampVector3(speed - speed * Time.deltaTime, new Vector3(float.MinValue, float.MinValue), new Vector3(0, float.MaxValue));
            else if (speed.x > 0)
                speed = VectorClamps.ClampVector3(speed - speed * Time.deltaTime, 0, float.MaxValue);
        }
    }

    private void Effects()
    {
        if (currentLifetime > maxLifetime * secondLifetimePart)
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
            sortingOrder--;
            this.gameObject.SetActive(false);
            popupPool.Enqueue(this.gameObject);
        }
    }
}
