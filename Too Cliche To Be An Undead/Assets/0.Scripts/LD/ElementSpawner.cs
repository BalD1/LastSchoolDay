using System.Text;
using UnityEngine;

public class ElementSpawner : MonoBehaviour
{
    public enum E_ElementToSpawn
    {
        RandomBaseZombie,
        Keycard,
        Coins,
        DashingZombie,
        CaCZombie,
        IdleZombie,
        GroundedZombie,
    }

    [SerializeField] private E_ElementToSpawn elementToSpawn;
    public E_ElementToSpawn ElementToSpawn { get => elementToSpawn; }

    [SerializeField] private bool destroyAfterSpawn;
    [SerializeField] private bool spawnAtStart = true;

    [SerializeField] private bool wasInstantiatedInRoom = false;

    [SerializeField] private bool flipX = false;

    public bool DestroyAfterSpawn { get => destroyAfterSpawn; }
    public bool SpawnAtStart { get => spawnAtStart; }

    public delegate void D_SpawnedKeycard(Keycard key);
    public D_SpawnedKeycard D_spawnedKeycard;

    private void Awake()
    {
        if (SpawnersManager.Instance == null) Destroy(this.gameObject);
    }

    public void Setup(E_ElementToSpawn _elementToSpawn, bool _destroyAfterSpawn, bool _spawnAtStart)
    {
        elementToSpawn = _elementToSpawn;
        destroyAfterSpawn = _destroyAfterSpawn;
        spawnAtStart = _spawnAtStart;
    }

    private void Start()
    {
        if (wasInstantiatedInRoom) SpawnersManager.Instance.AddSingleToArray(this.gameObject);
        if (spawnAtStart) SpawnElement();
    }

    public GameObject SpawnElement()
    {
        GameObject res = null;
        switch (elementToSpawn)
        {
            case E_ElementToSpawn.RandomBaseZombie:
                res = Spawn(GameAssets.Instance.GetRandomZombie, GameManager.Instance.InstantiatedEntitiesParent);
                break;

            case E_ElementToSpawn.Keycard:
                res = Spawn(GameAssets.Instance.KeycardPF, GameManager.Instance.InstantiatedKeycardsParent);
                D_spawnedKeycard?.Invoke(res.GetComponent<Keycard>());
                break;

            case E_ElementToSpawn.Coins:
                res = Spawn(GameAssets.Instance.CoinPF, GameManager.Instance.InstantiatedMiscParent);
                break;

            case E_ElementToSpawn.IdleZombie:
                res = NormalZombie.Create(this.transform.position, false).gameObject;
                res.transform.parent = GameManager.Instance.InstantiatedEntitiesParent;
                break;

            case E_ElementToSpawn.GroundedZombie:
                res = GroundedZombie.Create(this.transform.position, flipX).gameObject;
                res.transform.parent = GameManager.Instance.InstantiatedEntitiesParent;
                break;

            default:
                Debug.LogError(elementToSpawn + " not found in switch statement.");
                break;
        }

        if (destroyAfterSpawn)
        {
            Destroy(this.gameObject);
            if (elementToSpawn == E_ElementToSpawn.Keycard)
            {
                SpawnersManager.Instance.KeycardSpawners.Remove(this);
                SpawnersManager.Instance.ElementSpawners.Remove(this);
            }
            else SpawnersManager.Instance.ElementSpawners.Remove(this);
        }

        return res;
    }

    private GameObject Spawn(GameObject objPF, Transform parent)
    {
        GameObject instantiated = Instantiate(objPF, this.transform.position, Quaternion.identity);
        instantiated.transform.parent = parent;

        return instantiated;
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (this.gameObject.CompareTag("ElementSpawner") == false) this.gameObject.tag = "ElementSpawner";
#endif
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        CreateObjectName();
#endif
    }

    public void CreateObjectName()
    {
#if UNITY_EDITOR
        StringBuilder objectName = new StringBuilder("SPAWNER");
        switch (this.ElementToSpawn)
        {
            case ElementSpawner.E_ElementToSpawn.RandomBaseZombie:
                objectName.Append("_ZOM");
                break;

            case ElementSpawner.E_ElementToSpawn.Keycard:
                objectName.Append("_KEY");
                break;

            case ElementSpawner.E_ElementToSpawn.Coins:
                objectName.Append("_COIN");
                break;

            case ElementSpawner.E_ElementToSpawn.IdleZombie:
                objectName.Append("_IDLEZOM");
                break;

            case ElementSpawner.E_ElementToSpawn.GroundedZombie:
                objectName.Append("_GROUNDED");
                break;

            default:
                objectName.Append("_UNDEFINED");
                break;
        }

        objectName.Append(this.DestroyAfterSpawn ? "_DESTR" : "");
        objectName.Append(this.SpawnAtStart ? "_START" : "");

        this.gameObject.name = objectName.ToString();
#endif
    }
}
