using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PurpleIdleState : State
{
  PurplePlayer player;
  //float toiletTimer = 5f;

  public PurpleIdleState(PurplePlayer player) {
    this.player = player;
  }

    public override State Execute()
    {/*
        toiletTimer -= 1.0f;
        if (toiletTimer <= 0f)
        {
            State state = new BreakState();
            states.Push(this);
            return state;
        }
        else
        {*/
        //return new PurpleStopState(player);
        if (GameManager.Instance().IsGameOver() || GameManager.Instance().IsTimeOut())
        {
            return new PurpleStopState(player);
        }
        else
        {
            player.Wander();
            Debug.Log("Purple player " + player.playerID + " is searching for a new target !!");
            GreenPlayer target = GameManager.Instance().FindClosestTarget(player);

            if (target != null)
            {
                //Current logic on one purple can chase a green player
                if (!target.IsBeingChased() && !target.IsJailed())  //&& !target.captured
                {
                    //pass the info to the green player of being chased and change the status
                    target.BeingChased(player);
                    return new PurpleChasingState(player, target);
                }
            }

            return this;
        } 
    }
}
