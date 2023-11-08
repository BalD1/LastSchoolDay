using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerCharacter : NewEntity
{
    [field: SerializeField] public NewPlayerInputsHandler PlayerInputsComponent { get; private set; }
    [field: SerializeField] public int PlayerIndex {  get; private set; }

    protected override void EventsSubscriber()
    {
    }

    protected override void EventsUnSubscriber()
    {
    }

    public void Setup(NewPlayerInputsHandler playerInputs, int index)
    {
        this.PlayerInputsComponent = playerInputs;
        this.PlayerIndex = index;
    }
}
