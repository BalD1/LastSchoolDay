using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCharacter))]
public class PlayerStatsHandler : EntityStatsHandler<PlayerCharacter>
{
    private void Reset()
    {
        Owner = this.GetComponent<PlayerCharacter>();
    }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        Owner.OnCharacterSwitch += OnCharacterSwitch;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        Owner.OnCharacterSwitch -= OnCharacterSwitch;
    }

    private void OnCharacterSwitch(SO_CharactersComponents newCharacter)
    {
        base.ChangeBaseStats(newCharacter.Stats, false);
    }
}
