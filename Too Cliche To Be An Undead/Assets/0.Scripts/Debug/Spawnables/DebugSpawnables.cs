using System;
using System.Collections.Generic;
using UnityEngine;
using BalDUtilities.MouseUtils;

public class DebugSpawnables : MonoBehaviour
{
    [Serializable]
    public struct SpawnableByKey
    {
        public E_ScriptType scriptType;
        public E_SpawnPos spawnPos;
        public KeyCode key;
        public List<string> varsArgs;
        public Action action;

        public int count;

        public bool randomCount;
        public int minCount;
        public int maxCount;

        public int GetCount()
        {
            if (randomCount) return UnityEngine.Random.Range(minCount, maxCount);
            else return count;
        }

#if UNITY_EDITOR
        public bool showInEditor;
        public bool showArgsInEditor;
#endif


        public GameObject customPrefab;
        public Vector2 customPosition;
    }

    public enum E_ScriptType
    {
        Custom,
        HealthPopup,
        NormalZombie,
        Coin,
    }

    public enum E_SpawnPos
    {
        MousePosition,
        SelfPosition,
        CustomPosition,
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

                    for (int i = 0; i < s.GetCount(); i++)
                    {
                        Instantiate(s.customPrefab, pos, Quaternion.identity);
                    }
                });
                break;

            case E_ScriptType.HealthPopup:
                s.action = new Action(() =>
                {
                    Vector2 pos = GetPosBySpawnType(s);

                    for (int i = 0; i < s.GetCount(); i++)
                    {
                        HealthPopup.Create(pos, Convert.ToSingle(s.varsArgs[0]), Convert.ToBoolean(s.varsArgs[1]), Convert.ToBoolean(s.varsArgs[2]));
                    }
                });
                break;

            case E_ScriptType.NormalZombie:
                s.action = new Action(() =>
                {
                    Vector2 pos = GetPosBySpawnType(s);

                    for (int i = 0; i < s.GetCount(); i++)
                    {
                        NormalZombie.Create(pos, true);
                    }
                });
                break;

            case E_ScriptType.Coin:
                s.action = new Action(() =>
                {
                    Vector2 pos = GetPosBySpawnType(s);

                    for (int i = 0; i < s.GetCount(); i++)
                    {
                        GameObject gO = Instantiate(GameAssets.Instance.CoinPF, pos, Quaternion.identity);
                        gO.GetComponent<Coins>().coinValue = Convert.ToInt32(s.varsArgs[0]);
                    }
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
                pos = GameManager.Player1Ref.transform.position;
                break;

            case E_SpawnPos.CustomPosition:
                pos = s.customPosition;
                break;

            default:
                break;
        }

        return pos;
    }
}
