using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Purple players chase green players
public class PurplePlayer : Player
{
  StateMachine stateMachine;

  // The gaol we are taking the target to
  GameObject gaol = null;

    //To adjust the arrival speed
   // protected float targetRadius = 0.75f;
    protected float slowRadius = 2.0f;

   // public float captureRadius { get; }

  // Start overrides the baseclass Start, but uses it.
  protected override void Start()
  {
        //To spawn the purple players at random positions
    base.Start();
    
    stateMachine = new StateMachine(new PurpleIdleState(this));
    currentSpeed = 0.0f;
    gaol = gameManager.GetGaol();

  }

  // Update decides what to do, chase greens or bring them to gaol
  protected override void Update()
  {
    stateMachine.Execute();
    // Use the Move method of the parent class
    base.Update();
  }

  public bool Chase(GreenPlayer target)
  {
        //return MoveToAndAvoidObstacles(this.Position(), target.Position());
        return MoveTo(this.Position(), target.Position());
    }  

  // Take the prisoner to gaol and leave them there.  This method is incomplete.
  public bool TransportToGaol()
  {
    return MoveTo(this.Position(), new Vector2(gaol.transform.position.x, gaol.transform.position.z));
  }


}
