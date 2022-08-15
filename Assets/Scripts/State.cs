using System.Collections;
using System.Collections.Generic;

public abstract class State 
{
  protected Stack<State> states = new Stack<State>();


  public abstract State Execute();
}
