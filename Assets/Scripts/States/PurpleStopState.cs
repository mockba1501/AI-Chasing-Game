using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleStopState : State
{
    PurplePlayer player;

    public PurpleStopState(PurplePlayer player)
    {
        this.player = player;
    }

    public override State Execute()
    {
        player.StopMovement();
        return this;

    }
}
