using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Smokescreen : SkillBase
{
    public override void Setup(GameObject _owner)
    {
        this.owner = _owner;
    }

    public override void Use()
    {
        animator.Play("start");
    }


    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (debugMode)
        {
            Gizmos.DrawWireSphere(this.transform.position, SkillRange);

        }
#endif
    }
}
