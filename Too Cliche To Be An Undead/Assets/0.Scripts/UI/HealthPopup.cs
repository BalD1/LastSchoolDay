using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BalDUtilities.VectorUtils;

public class HealthPopup : MonoBehaviour
{
    [System.Serializable]
    public struct HitComponents
    {
        public Vector3 speedMovements;
        public float lifetime;
        public float disapearSpeed;
        public float fontSize;
        public float increaseScaleAmount;
        public float decreaseScaleAmount;
        public Color color;
    }

    [SerializeField] private HitComponents normalHit;
    [SerializeField] private HitComponents criticalHit;

    [SerializeField] private HitComponents normalHeal;
    [SerializeField] private HitComponents criticalHeal;

    private float maxLifetime;

    private HitComponents componentsNeeded;

    private TextMeshPro textMesh;

    private Color textColor;

    private static int sortingOrder;

    private const float secondLifetimePart = .7f;

    public static Queue<GameObject> popupPool = new Queue<GameObject>();

    public static HealthPopup Create(Vector3 position, float amount, bool isHeal, bool isCritialHit = false)
    {
        GameObject healthPopupGO;

        if (popupPool.Count > 0)
        {
            healthPopupGO = popupPool.Dequeue();

            if (healthPopupGO == null)
            {
                healthPopupGO = Instantiate(GameAssets.Instance.DamagesPopupPF, position, Quaternion.identity);
            }

            healthPopupGO.transform.position = position;
            healthPopupGO.transform.localScale = Vector3.one;
        }
        else healthPopupGO = Instantiate(GameAssets.Instance.DamagesPopupPF, position, Quaternion.identity);

        HealthPopup healthpopup = healthPopupGO.GetComponent<HealthPopup>();

        healthpopup.Setup(amount, isHeal, isCritialHit);

        healthPopupGO.SetActive(true);

        return healthpopup;
    }

    public void Setup(float amount, bool isHeal, bool isCritialHit = false)
    {
        if (isHeal) componentsNeeded = isCritialHit ? criticalHeal : normalHeal;
        else componentsNeeded = isCritialHit ? criticalHit : normalHit;

        textMesh.SetText(amount.ToString());

        textMesh.fontSize = componentsNeeded.fontSize;
        textMesh.color = componentsNeeded.color;

        maxLifetime = componentsNeeded.lifetime;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

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
            sortingOrder--;

            this.gameObject.SetActive(false);
            popupPool.Enqueue(this.gameObject);
        }
    }

}