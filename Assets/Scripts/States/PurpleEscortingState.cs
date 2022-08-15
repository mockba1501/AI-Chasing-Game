using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleEscortingState : State
{
  GreenPlayer target;
  PurplePlayer player;

  public PurpleEscortingState(PurplePlayer player, GreenPlayer target)
  {
    this.player = player;
    this.target = target;
  }

  public override State Execute()
  {
        //Go to the Goal
        player.TransportToGaol();
        //Check if the player reaches the jail
        if (target.IsJailed())
        {
            //Green player reaches jail
            player.ReAdjustMaximumSpeed();
            return new PurpleIdleState(player);
        }
        else
        {
            Debug.Log("Purple player " + player.playerID + " escorting green player " + target.playerID + " to jail");
            return this;
        }
  }
}
