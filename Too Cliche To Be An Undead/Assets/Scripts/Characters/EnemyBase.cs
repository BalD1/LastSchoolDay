using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : Entity
{
    [SerializeField] private SCRPT_DropTable dropTable;
    
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);

        if (dropTable != null)
        {
            GameObject drop = dropTable.GetRandomDrop();
            if (drop != null)
            {
                Instantiate(drop, transform.position, Quaternion.identity);
            }
        }
    }
}
