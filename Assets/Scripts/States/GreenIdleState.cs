using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenIdleState : State
{
    GreenPlayer player;

    public GreenIdleState(GreenPlayer player)
    {
        this.player = player;
    }

    public override State Execute()
    {
        if (GameManager.Instance().IsGameOver() || GameManager.Instance().IsTimeOut())
        {
            return new GreenStopState(player);
        }
        if (!player.IsBeingChased())
        {
            //player.CompositeMove();
            //player.RandomMove();
            player.Wander();
            return this;
        }
        else
        {
              return new GreenBeingChasedState(player,player.GetChaser());
        }
    }
}
