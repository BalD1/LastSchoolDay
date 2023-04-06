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
    }

    [SerializeField] private E_ElementToSpawn elementToSpawn;
    public E_ElementToSpawn ElementToSpawn { get => elementToSpawn; }

    [SerializeField] private bool destroyAfterSpawn;
    [SerializeField] private bool spawnAtStart = true;

    [SerializeField] private bool wasInstantiatedInRoom = false;

    public bool DestroyAfterSpawn { get => destroyAfterSpawn; }
    public bool SpawnAtStart { get => spawnAtStart; }

    public delegate void D_SpawnedKeycard(Keycard key);
    public D_SpawnedKeycard D_spawnedKeycard;

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

    public void SpawnElement()
    {
        switch (elementToSpawn)
        {
            case E_ElementToSpawn.RandomBaseZombie:
                Spawn(GameAssets.Instance.GetRandomZombie, GameManager.Instance.InstantiatedEntitiesParent);
                break;

            case E_ElementToSpawn.Keycard:
                Keycard key = Spawn(GameAssets.Instance.KeycardPF, GameManager.Instance.InstantiatedKeycardsParent).GetComponent<Keycard>();
                D_spawnedKeycard?.Invoke(key);
                break;

            case E_ElementToSpawn.Coins:
                Spawn(GameAssets.Instance.CoinPF, GameManager.Instance.InstantiatedMiscParent);
                break;

            case E_ElementToSpawn.IdleZombie:
                NormalZombie.Create(this.transform.position, false).transform.parent = GameManager.Instance.InstantiatedEntitiesParent;
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
