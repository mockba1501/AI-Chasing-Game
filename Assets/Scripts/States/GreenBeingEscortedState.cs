using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBeingEscortedState : State
{
    private GreenPlayer player;
    private PurplePlayer purpleEscorter;

    public GreenBeingEscortedState(GreenPlayer player, PurplePlayer chaser)
    {
        this.player = player;
        this.purpleEscorter = chaser;
    }
    public override State Execute()
    {
        if(!player.EscortToGaol())
        {
            //Move player along with the purple player to jail
            
            return this;
        }
        else
        {

            //if player reaches jail then stop green player (Destroy)    
            GameManager.Instance().RemoveGreenPlayer(player);
            return new GreenStopState(player);

            
        }
    }
}
