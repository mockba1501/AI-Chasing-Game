using System.Collections;
using System.Collections.Generic;

public class BreakState : State
{

  public override State Execute()
  {
    // Introduce a delay of 10 seconds ... 
    return states.Pop();
  }
}
