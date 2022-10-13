using System;
using System.Collections.Generic;
using UnityEngine;
using BalDUtilities.MouseUtils;

public class DebugSpawnables : MonoBehaviour
{
#if UNITY_EDITOR
    [Serializable]
    public struct SpawnableByKey
    {
        public E_ScriptType scriptType;
        public E_SpawnPos spawnPos;
        public KeyCode key;
        public List<string> varsArgs;
        public Action action;

        public bool showInEditor;
        public bool showArgsInEditor;

        public GameObject customPrefab;
    }

    public enum E_ScriptType
    {
        Custom,
        HealthPopup,
        TrainingDummy,
        Coin,
    }

    public enum E_SpawnPos
    {
        MousePosition,
        SelfPosition,
    }

    public List<SpawnableByKey> spawnableByKey;

    private void Awake()
    {
        for (int i = 0; i < spawnableByKey.Count; i++)
        {
            spawnableByKey[i] = CreateActionBasedOnType(spawnableByKey[i]);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (var item in spawnableByKey)
            {
                if (Input.GetKeyDown(item.key))
                    item.action?.Invoke();
            }
        }
    }

    public static SpawnableByKey CreateActionBasedOnType(SpawnableByKey spawnable)
    {
        SpawnableByKey s = spawnable;
        switch (s.scriptType)
        {
            case E_ScriptType.Custom:
                s.action = new Action(() =>
                {
                    Vector2 pos = GetPosBySpawnType(s);
                    Instantiate(s.customPrefab, pos, Quaternion.identity);
                });
                break;

            case E_ScriptType.HealthPopup:
                s.action = new Action(() =>
                {
                    Vector2 pos = GetPosBySpawnType(s);
                    HealthPopup.Create(pos, Convert.ToSingle(s.varsArgs[0]), Convert.ToBoolean(s.varsArgs[1]), Convert.ToBoolean(s.varsArgs[2]));
                });
                break;

            case E_ScriptType.TrainingDummy:
                s.action = new Action(() =>
                {
                    Vector2 pos = GetPosBySpawnType(s);
                    TrainingDummy.Create(pos, Convert.ToSingle(s.varsArgs[0]));
                });
                break;

            case E_ScriptType.Coin:
                s.action = new Action(() =>
                {
                    Vector2 pos = GetPosBySpawnType(s);
                    GameObject gO = Instantiate(GameAssets.Instance.CoinPF, pos, Quaternion.identity);
                    gO.GetComponent<Coins>().coinValue = Convert.ToInt32(s.varsArgs[0]);
                });
                break;

            default:
                break;
        }

        return s;
    }

    private static Vector2 GetPosBySpawnType(SpawnableByKey s)
    {
        Vector2 pos = Vector2.zero;
        switch (s.spawnPos)
        {
            case E_SpawnPos.MousePosition:
                pos = MousePosition.GetMouseWorldPosition();
                break;
            case E_SpawnPos.SelfPosition:
                pos = GameManager.PlayerRef.transform.position;
                break;
            default:
                break;
        }

        return pos;
    }
#endif
}
