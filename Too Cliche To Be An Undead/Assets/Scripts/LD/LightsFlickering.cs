using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightsFlickering : MonoBehaviour
{
    [SerializeField] private Light2D lightComp;

    [SerializeField] private int flickeringChances = 25;

    private float t0;

    #region Intensity

    [Header("Intensity")]

    [SerializeField] private bool flickIntensity;

    [SerializeField] private float intensityRange = 0.14f;

    [SerializeField] private float intensityTimeMin = 0.05f;
    [SerializeField] private float intensityTimeMax = 0.1f;

    private float intensityTimer;

    private float baseIntensity;

    #endregion

    #region Position

    [Header("Position")]

    [SerializeField] private bool flickPosition;

    [SerializeField] private float positionRadius = 0.01f;
    [SerializeField] private float angle = 40;

    [SerializeField] private float positionTimeMin = 0.05f;
    [SerializeField] private float positionTimeMax = 0.1f;

    private float positionTimer;

    #endregion

    #region Color

    [Header("Color")]

    [SerializeField] private bool flickColor;
    [SerializeField][Range(0, 255)] private float colorRadius = 0.01f;

    [SerializeField] private float colorTimeMin = 0.05f;
    [SerializeField] private float colorTimeMax = 0.5f;

    private float colorTimer;

    private Color color;
    private Color baseColor;
    private Vector3 colorVector;

    private Vector3 shift;

    private Vector3 basePosition; 

    #endregion

    private void Awake()
    {
        int roll = Random.Range(0, 100);

        if (roll > flickeringChances) Destroy(this);

        t0 = Time.time;
        intensityTimer = positionTimer = colorTimer = t0;

        baseIntensity = lightComp.intensity;

        shift = Vector3.zero;
        basePosition = this.transform.position;

        baseColor = lightComp.color;
    }

    private void Update()
    {
        t0 = Time.time;

        if (flickIntensity) FlickIntensity();
        if (flickPosition) FlickPosition();
        if (flickColor) FlickColor();
    }

    #region Flickerings

    private void FlickIntensity()
    {
        if (intensityTimer > 0)
        {
            intensityTimer -= Time.deltaTime;
            return;
        }
        intensityTimer = Random.Range(intensityTimeMin, intensityTimeMax);

        float r = Random.Range(baseIntensity - intensityRange, baseIntensity + intensityRange);
        if (r <= 0) r = 0;
        lightComp.intensity = r;
    }

    private void FlickPosition()
    {
        if (positionTimer > 0)
        {
            positionTimer -= Time.deltaTime;
            return;
        }
        positionTimer = Random.Range(positionTimeMin, positionTimeMax);

        float r = Random.Range(0f, positionRadius);
        float theta = Random.Range(0.5f * (Mathf.PI - angle * Mathf.Deg2Rad),
                                   0.5f * (Mathf.PI + angle * Mathf.Deg2Rad));
        shift.x = r * Mathf.Cos(theta);
        shift.y = r * Mathf.Sin(theta);
        transform.position = basePosition + shift;
    }

    private void FlickColor()
    {
        if (colorTimer > 0)
        {
            colorTimer -= Time.deltaTime;
            return;
        }
        colorTimer = Random.Range(colorTimeMin, colorTimeMax);

        Vector3ToColor(Random.insideUnitSphere * colorRadius + colorVector);
        color.a = 1;
        lightComp.color = color;
    }

    private void Vector3ToColor(Vector3 v)
    {
        color.r = v.x;
        color.g = v.y;
        color.b = v.z;
    }

    #endregion

    public void StartIntensityFlick() => flickIntensity = true;
    public void StartPositionFlick() => flickPosition = true;
    public void StartColorFlick() => flickColor = true;

    public void StopIntensityFlick()
    {
        flickIntensity = false;
        lightComp.intensity = baseIntensity;
        intensityTimer = 0;
    }
    public void StopPositionFlick()
    {
        flickPosition = false;
        this.transform.position = basePosition;
        positionTimer = 0;
    }
    public void StopColorFlick()
    {
        flickColor = false;
        lightComp.color = baseColor;
        colorTimer = 0;
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (lightComp == null) lightComp = this.GetComponent<Light2D>();
#endif
    }
}
