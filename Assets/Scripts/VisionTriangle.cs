using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class for describing a sector in which a player can see
// This is probably more efficient as a struct (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/struct)
public class VisionTriangle
{
  public float minimumAngle = -45f;
  public float maximumAngle = 45f;
  public float maximumDistance = 30f;

  public VisionTriangle()
  {

  }

 public VisionTriangle(float totalAngle, float maximumDistance)
  {
    minimumAngle = -totalAngle / 2f;
    maximumAngle = totalAngle / 2f;
    this.maximumDistance = maximumDistance;
  }

  public VisionTriangle(float minimumAngle, float maximumAngle, float maximumDistance)
  {
    this.minimumAngle = minimumAngle;
    this.maximumAngle = maximumAngle;
    this.maximumDistance = maximumDistance;
  }
}
