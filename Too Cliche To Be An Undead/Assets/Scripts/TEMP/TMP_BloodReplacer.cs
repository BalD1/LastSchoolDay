using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class TMP_BloodReplacer : MonoBehaviour
{
    [SerializeField] private GameObject[] bloodPrefabs;

    [InspectorButton(nameof(ReplaceBlood), ButtonWidth = 150)]
    public bool replace;

    private void ReplaceBlood()
    {
        GameObject[] bloods = GameObject.FindGameObjectsWithTag("Blood");

        for (int i = 0; i < bloods.Length; i++)
        {
            SpriteRenderer sr = bloods[i].GetComponent<SpriteRenderer>();

            if (sr == null) continue;

            foreach (var pf in bloodPrefabs)
            {
                if (sr.sprite == pf.GetComponent<SpriteRenderer>().sprite)
                {
                    GameObject gO = PrefabUtility.InstantiatePrefab(pf, bloods[i].transform.parent) as GameObject;
                    gO.transform.position = bloods[i].transform.position;
                    gO.transform.rotation = bloods[i].transform.rotation;
                    gO.transform.localScale = bloods[i].transform.localScale;

                    DestroyImmediate(bloods[i], true);

                    break;
                }
            }
        }
    }
}
