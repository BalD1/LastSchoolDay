using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_loot : MonoBehaviour
{
    [SerializeField] private SCRPT_DropTable dropTable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) dropTable.DropRandom(this.transform.position);
    }
}
