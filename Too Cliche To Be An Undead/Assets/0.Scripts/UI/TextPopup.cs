using BalDUtilities.VectorUtils;
using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    [SerializeField] [ReadOnly] private float maxLifetime;
    [SerializeField][ReadOnly] private float currentLifetime;

    [SerializeField] [ReadOnly] private Vector3 speed;

    [SerializeField] [ReadOnly] private SCRPT_TextPopupComponents.HitComponents componentsNeeded;

    private TextMeshPro textMesh;

    private Color textColor;

    private bool simulate = false;

    private bool allowMovements = true;

    private const float secondLifetimePart = .5f;
    private static int sortingOrder;

    private static MonoPool<TextPopup> popupPool;

    public static void CheckPool()
    {
        if (popupPool == null)
            popupPool = new MonoPool<TextPopup>
                (_createAction: () => GameAssets.Instance.TextPopupPF.Create(Vector2.zero).GetComponent<TextPopup>(),
                _parentContainer: GameManager.Instance.InstantiatedMiscParent);
    }

    #region Create

    /// <summary>
    /// Create or pools a new "<b><see cref="TextPopup"/></b>" displaying "<b><paramref name="text"/></b>" at "<b><paramref name="pos"/></b>" position.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static TextPopup Create(string text, Vector2 pos, bool allowMovements = true) => Create(text, pos, GameAssets.BaseComponents, allowMovements);
    /// <summary>
    /// Create or pools a new "<b><see cref="TextPopup"/></b>" displaying "<b><paramref name="text"/></b>".
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static TextPopup Create(string text, Transform parent, bool allowMovements = true) => Create(text, parent, GameAssets.BaseComponents, allowMovements);
    /// <summary>
    /// Create or pools a new "<b><see cref="TextPopup"/></b>" displaying "<b><paramref name="text"/></b>" at "<b><paramref name="pos"/></b>" position and with "<b><paramref name="components"/></b>" style.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pos"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    public static TextPopup Create(string text, Vector2 pos, SCRPT_TextPopupComponents.HitComponents components, bool allowMovements = true)
    {
        TextPopup txtPopup = popupPool.GetNext(pos);
        txtPopup.Setup(text, components);
        txtPopup.allowMovements = allowMovements;
        return txtPopup;
    }
    /// <summary>
    /// Create or pools a new "<b><see cref="TextPopup"/></b>" displaying "<b><paramref name="text"/></b>" and with "<b><paramref name="components"/></b>" style.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static TextPopup Create(string text, Transform parent, SCRPT_TextPopupComponents.HitComponents components, bool allowMovements = true)
    {
        TextPopup txtPopup = popupPool.GetNext();
        txtPopup.Setup(text, components);
        txtPopup.transform.SetParent(parent, false);
        txtPopup.allowMovements = allowMovements;
        return txtPopup;
    }

    #endregion

    public void Setup(string text) => Setup(text, GameAssets.BaseComponents);
    public void Setup(string text, SCRPT_TextPopupComponents.HitComponents components)
    {
        this.transform.localScale = Vector3.one;
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

        simulate = true;
    }

    private void Awake()
    {
        textMesh = this.transform.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        if (!simulate) return;

        Movements();
        Effects();
        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0)
            Disappear();
    }

    private void Movements()
    {
        if (!allowMovements) return;
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
            popupPool.Enqueue(this);
            simulate = false;
        }
    }
}
