using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenStopState : State
{
    GreenPlayer player;

    public GreenStopState(GreenPlayer player)
    {
        this.player = player;
    }

    public override State Execute()
    {
        player.StopMovement();
        return this;

    }
}