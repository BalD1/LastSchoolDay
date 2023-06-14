using UnityEngine;

public static class FSM_Entity_Extensions
{
    public static Vector2 GetPushForce(this FSM_ManagerBase fsm, Entity self, float force, Entity pusher, Entity originalPusher)
    {
        Vector2 pusherPos = pusher.transform.position;
        Vector2 selfPos = self.transform.position;

        float dist = Vector2.Distance(pusherPos, selfPos) / 2;
        Vector2 dir = (selfPos - pusherPos).normalized;
        DashHitParticles.GetNext(pusherPos + (dist * dir));

        float finalForce = force - self.GetStats.Weight;
        if (finalForce <= 0) return Vector2.zero;

        Vector2 v = dir * finalForce;
        return v;
    }
}
