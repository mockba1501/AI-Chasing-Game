using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The player class contains the common features for green and purple players
public abstract class Player : MonoBehaviour
{
    private Vector2 wanderTarget;
    // Where is the player
    protected Vector2 position;
    // What is the maximum speed of the player?
    protected float maxSpeed; // (Randomize between 0.1f and 0.3f)
    // Store the local max value to be readjusted after slow down 
    protected float maxSpeedSave;
    // What is the maximum rotation speed of the player?
    protected float maxRotationSpeed;  // (Randomize between 10f and 30f)
    // What is the current speed of the player?
    protected float currentSpeed = 0.01f;
    // What is the current direction (rotation around the y axis) of the player?
    protected float currentRotation = 45f;

    protected float targetRadius = 0.55f;
    protected float captureRadius = 0.1f;
    protected float obstacleAvoidanceDistance = 1.0f;

    protected GameManager gameManager;

    public int playerID;

    // The purpose of these Dictionaries (see https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-6.0)
    // is to keep track of where this player last saw the other players.
    // The Dictionary is indexed by a player, and returns a timestamp.
    //protected Dictionary<Player, float> greenPlayers = new Dictionary<Player, float>();
    //protected Dictionary<Player, float> purplePlayers = new Dictionary<Player, float>();
    protected List<Player> greenPlayers = new List<Player>();
    protected List<Player> purplePlayers = new List<Player>();

    // A string to identify objects tagged as obstacles
    const string ObstacleTag = "Obstacle";
    // We assume all obstacles have a fixed diameter to simplify processing
    const float obstacleDiameter = 2.0f;
    // We set the field of view to 90°
    const float viewAngleHalf = 45f;
    // We ignore = cannot see players further away than 30 units
    const float viewDistance = 30f;

    //For repulsion and attraction models
    const float repulsionGeneral = 0.5f;
    const float attractionGeneral = 2.0f;

    // We store all obstacles
    protected List<GameObject> obstacles;

    // We expect the subclasses to have their own implementations of Start
    protected virtual void Start()
  {
    gameManager = GameManager.Instance();

        // Set our local understanding of the position from the Unity position
        position = new Vector2(transform.position.x, transform.position.z);
        maxRotationSpeed = Random.Range(10f, 30f);
        maxSpeed = Random.Range(0.01f, 0.1f);
        maxSpeedSave = maxSpeed;


        // Find the obstacles
        obstacles = new List<GameObject>(GameObject.FindGameObjectsWithTag(ObstacleTag));
        // Locate all players and store them (which includes their position) along with a time stamp for when they were last seen
        foreach (Player green in gameManager.GreenPlayers())
            greenPlayers.Add(green); //, Time.time);
        foreach (Player purple in gameManager.PurplePlayers())
            purplePlayers.Add(purple); //, Time.time);
    }

  // Update can be overridden too.
  protected virtual void Update()
  {
        if (gameManager.CheckPlayerDistanceWithinBoundaries(this.Position()))
            Move();
        else
            GoToCenter();

    }

    // We could alternatively implement Position as a property (see https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties)
    public Vector2 Position()
  {
    return position;
  }

  // Move the player according to its current speed and direction
  protected void Move()
  {
    position += new Vector2(currentSpeed * Mathf.Cos(currentRotation), currentSpeed * Mathf.Sin(currentRotation));

    transform.rotation = Quaternion.Euler(0.0f, currentRotation, 0.0f);
    transform.position = new Vector3(position.x, 0.0f, position.y);
  }

    protected bool MoveTo(Vector2 startingPoint, Vector2 endPoint)
    {
        //Vector2 movementSum = AvoidObstacles();
        Vector2 direction = endPoint - startingPoint;
        //movementSum += (endPoint - startingPoint) * attractionGeneral;
        float distance = direction.magnitude;
        //float distance = movementSum.magnitude;

        float futureRotation = Mathf.Atan2(direction.y, direction.x);
        //float futureRotation = Mathf.Atan2(movementSum.y, movementSum.x);
        
        // Limit the rotation and linear speeds
        currentRotation += Mathf.Clamp(futureRotation - currentRotation, -maxRotationSpeed, maxRotationSpeed);
        currentSpeed = Mathf.Clamp(distance, 0.0f, maxSpeed);

        //Debug.Log(distance + " " + currentSpeed * Time.deltaTime);
        return (distance <= currentSpeed);

    }

    protected Vector2 AvoidObstacles()
    {
        Vector2 movementSum = Vector2.zero;
        Vector2 direction;
        float distance;
        

        foreach (GameObject obstacle in obstacles)
        {
            Vector2 obstaclePosition = new Vector2(obstacle.transform.position.x, obstacle.transform.position.z);
            direction = obstaclePosition - this.Position();
            distance = direction.magnitude;

            movementSum -= repulsionGeneral * direction / distance;
        }

        foreach (GameObject wall in gameManager.Fences())
        {
            Vector2 wallPosition = new Vector2(wall.transform.position.x, wall.transform.position.z);
            direction = wallPosition - this.Position();
            distance = direction.magnitude;

            movementSum -= repulsionGeneral * direction / distance;

        }
        
        return movementSum;
        /*
        currentSpeed = Mathf.Clamp(movementSum.magnitude, 0.0f, maxSpeed);
        Debug.Log("Composite current speed " + currentSpeed);

        float futureRotation = Mathf.Atan2(movementSum.y, movementSum.x);
        Debug.Log("Composite current rotation " + currentRotation);

        // Limit the rotation and linear speeds
        currentRotation += Mathf.Clamp(futureRotation - currentRotation, -maxRotationSpeed, maxRotationSpeed);
        */
    }

    protected bool MoveToAndAvoidObstacles(Vector2 startingPoint, Vector2 endPoint)
    {
        Vector2 movementSum = Vector2.zero;
        Vector2 direction;
        float distance;
        float repulsionGeneral = 0.5f;
        

        direction = endPoint - startingPoint;
        distance = direction.magnitude;

        //Local scope For purple player
        direction = startingPoint - endPoint;
        //distance = direction.magnitude;

        movementSum += attractionGeneral * direction;


        foreach (GameObject obstacle in obstacles)
        {
            Vector2 obstaclePosition = new Vector2(obstacle.transform.position.x, obstacle.transform.position.z);
            direction = obstaclePosition - this.Position();
            distance = direction.magnitude;

            movementSum -= repulsionGeneral * direction / distance;
        }

        foreach (GameObject wall in gameManager.Fences())
        {
            Vector2 wallPosition = new Vector2(wall.transform.position.x, wall.transform.position.z);
            direction = wallPosition - this.Position();
            distance = direction.magnitude;

            movementSum -= repulsionGeneral * direction / distance;

        }

        currentSpeed = Mathf.Clamp(movementSum.magnitude, 0.0f, maxSpeed);
        Debug.Log("Composite current speed " + currentSpeed);

        float futureRotation = Mathf.Atan2(movementSum.y, movementSum.x);
        Debug.Log("Composite current rotation " + currentRotation);


        //float futureRotation = Mathf.Atan2(direction.y, direction.x);

        // Limit the rotation and linear speeds
        currentRotation += Mathf.Clamp(futureRotation - currentRotation, -maxRotationSpeed, maxRotationSpeed);
        
        return MoveTo(startingPoint, endPoint);

    }

    // Moving the player based on attraction and repulsion from other players.  We should also consider obstacles and boundaries
    protected virtual void Move(List<Player> attractivePlayers, List<Player> repulsivePlayers, List<GameObject> obstacles)
    {

        Vector2 movementSum = Vector2.zero;

        float attraction = 3.0f;
        float repulsion = 0.5f;

        // We use the list of players we have stored.
        // The attraction of a player is attenuated by both the distance to it and the elapsed time since that player was last seen.
        // (The attenuation is going to be quite strong this way, and may need to be adjusted.)
        if (attractivePlayers != null)
        {
            foreach (Player p in attractivePlayers)
            {
                Vector2 direction = p.Position() - this.Position();
                float distance = direction.magnitude;
                //float elapsedTime = Time.time - p.Value;

                movementSum += attraction * direction / distance; // / elapsedTime;
            }
        }
        // We treat repulsive players (i e, members of our own team) the same way.
        if (repulsivePlayers != null)
        {
            foreach (Player p in repulsivePlayers)
            {
                Vector2 direction = p.Position() - this.Position();
                float distance = direction.magnitude;
                //float elapsedTime = Time.time - p.Value;

                movementSum -= repulsion * direction / distance; // / elapsedTime;
            }
        }

        // Missing: Processing of obstacles and boundaries.  They work the same way as repulsive players, but since they don’t move,
        // we do not apply a time attenuation to them.

        foreach (GameObject obstacle in obstacles)
        {
            Vector2 obstaclePosition = new Vector2(obstacle.transform.position.x, obstacle.transform.position.z);
            Vector2 direction = obstaclePosition - this.Position();
            float distance = direction.magnitude;

            movementSum -= repulsion * direction / distance;
        }
        Debug.Log("Movemenet Sum is " + movementSum);

        currentSpeed = Mathf.Clamp(movementSum.magnitude, 0.0f, maxSpeed);
        Debug.Log("Composite current speed " + currentSpeed);

        currentRotation = Mathf.Atan2(movementSum.y, movementSum.x);
        Debug.Log("Composite current rotation " + currentRotation);
        Move();
    }
    public void StopMovement()
    {
        currentRotation = 0.0f;
        currentSpeed = 0.0f;
    }

    public void SlowDown()
    {
        //Adjust the maximum speed to give a slow down effect
        maxSpeed = 0.03f;
    }

    public void ReAdjustMaximumSpeed()
    {
        maxSpeed = maxSpeedSave;
    }    

    /*
    public bool WallNearby()
    {

        foreach (GameObject wall in gameManager.Fences())
        {
            Vector2 wallPosition = new Vector2(wall.transform.position.x, wall.transform.position.z);
            float distance = wallPosition.magnitude;
            if (distance <= obstacleAvoidanceDistance)
            {
                return true;
            }
        }
        return false;
    }
    */
    //Associated with the Idle movement states of both players
    public void Wander()
    {
        float wanderRadius = 10;
        float wanderDistance = 20;
        float wanderJitter = 3;

        wanderTarget += new Vector2(Random.Range(-1.0f, 1.0f) * wanderJitter, Random.Range(-1.0f, 1.0f) * wanderJitter);

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector2 targetLocal = wanderTarget + new Vector2(0, wanderDistance);
        Vector2 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

        MoveTo(this.Position(), targetWorld);
    }

    protected void GoToCenter()
    {
        MoveTo(this.Position(), new Vector2(0, 0));
        Move();
    }
}
