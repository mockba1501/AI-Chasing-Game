using System.Collections;
using System.Collections.Generic;

public class StateMachine 
{
  State currentState = null;

  public StateMachine(State startState)
  {
    currentState = startState;

  }

  public void Execute()
  {
    if (currentState != null)
      currentState = currentState.Execute();
  }
}
