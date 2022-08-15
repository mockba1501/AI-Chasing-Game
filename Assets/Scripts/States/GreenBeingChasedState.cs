using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBeingChasedState : State
{
    private GreenPlayer player;
    private PurplePlayer purpleChaser;

    public GreenBeingChasedState(GreenPlayer player, PurplePlayer chaser)
    {
        this.player = player;
        this.purpleChaser = chaser;
    }
    public override State Execute()
    {
        if (GameManager.Instance().IsGameOver() || GameManager.Instance().IsTimeOut())
        {
            return new GreenStopState(player);
        }
        if (!player.IsCaptured())
        {
            player.Evade();
            return this;
        }
        else
            return new GreenBeingEscortedState(player, purpleChaser);
    }

}
