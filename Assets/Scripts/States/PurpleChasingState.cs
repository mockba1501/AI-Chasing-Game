using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleChasingState : State
{
  private GreenPlayer target = null;
  private PurplePlayer player;

  public PurpleChasingState(PurplePlayer player, GreenPlayer target)
  {
    this.player = player;
    this.target = target;
  }

  public override State Execute()
  {
        if(GameManager.Instance().IsGameOver() || GameManager.Instance().IsTimeOut())
        {
            return new PurpleStopState(player);
        }
        else if (player.Chase(target))
        {
           // target.captured = true;
            target.Captured(player);
            
            //To slow down the purple player while escorting the green player
            player.SlowDown();
            Debug.Log("Purple player " + player.playerID + " Captured green Player! " + target.playerID);
            return new PurpleEscortingState(player, target);
        }
        else
        {
            Debug.Log("Purple player " + player.playerID + " Chasing green Player! " + target.playerID);
            return this;
        }
  }

}
