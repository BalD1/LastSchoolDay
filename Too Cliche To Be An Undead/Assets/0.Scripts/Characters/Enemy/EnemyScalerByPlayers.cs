using System.Text;
using UnityEngine;

public class EnemyScalerByPlayers : MonoBehaviour
{
    [System.Serializable]
    public struct S_StatScaleAmount
    {
        public StatsModifier.E_StatType statToScale;
        public AnimationCurve scaleByPlayersCount;
    }

    [field: SerializeField] public S_StatScaleAmount[] StatsToScale { get; private set; }

    [SerializeField] protected EnemyBase owner;

    public const string MODIFIER_ID = "Multiplayer_Scale_";

    private void Reset()
    {
        owner = this.GetComponentInParent<EnemyBase>();
    }

    protected virtual void Start()
    {
        int playersCount = GameManager.Instance.PlayersCount;

        if (playersCount <= 1 || owner == null)
        {
            Destroy(this);
            return;
        }

        foreach (var item in StatsToScale)
        {
            string id = BuildModifierID(item.statToScale);
            float multiplier = item.scaleByPlayersCount[playersCount].value;

            float finalAmount = owner.GetStats.GetStatValue(item.statToScale) * multiplier;
            owner.AddModifier(id, finalAmount, item.statToScale);
        }
    }

    private string BuildModifierID(StatsModifier.E_StatType statType)
    {
        StringBuilder sb = new StringBuilder(MODIFIER_ID);
        sb.Append(statType.ToString());

        return sb.ToString();
    }
}
