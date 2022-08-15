using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The green players get chased
public class GreenPlayer : Player
{
    // The player should know if it has been captured, in which case it should follow its captor to gaol
    StateMachine stateMachine;

    GameObject gaol = null;
    PurplePlayer purpleChaser = null;

    //Vector2 wanderTarget = Vector2.zero;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        stateMachine = new StateMachine(new GreenIdleState(this));
        gaol = gameManager.GetGaol();
    }

    // Update is called once per frame
    protected override void Update()
    {
        stateMachine.Execute();

        base.Update();
    }

    public void RandomMove()
    {
        float offset = (Random.value - Random.value) / 5;

        currentRotation += (Mathf.Clamp(offset, 0.0f, maxRotationSpeed));
    }



    public bool IsBeingChased()
    {
        //Check the status of the current green player
        return purpleChaser != null;
    }

    public void BeingChased(PurplePlayer chaser)
    {
        //beingChased = true;
        this.purpleChaser = chaser;

    }

    //Moves opposite direction from the purple player
    public void Evade()
    {
        //CompositeMove();
        
        MoveAway(this.Position(), this.purpleChaser.Position());

        //MoveAwayFromObstacles();

        //MoveAwayAndAvoidObstacles(this.Position(), this.purpleChaser.Position());
    }


    public PurplePlayer GetChaser()
    {
        return purpleChaser;
    }



    public void Captured(PurplePlayer capturer)
    {
        GameManager.Instance().GreenPlayerCaptured();
    }

    public bool IsCaptured()
    {
        if (Vector2.Distance(this.Position(), this.purpleChaser.Position()) <= captureRadius)
        {
            //GameManager.Instance().GreenPlayerCaptured();
            return true;
        }
        else return false;

    }
    public bool EscortToGaol()
    {
        //Follow the purple player escort to the jail
        MoveTo(this.Position(), purpleChaser.Position());


        return IsJailed();
    }

    public bool IsJailed()
    {
        //Check if green player reached jail or not
        Vector2 direction = new Vector2(gaol.transform.position.x, gaol.transform.position.z) - this.Position();
        if (direction.magnitude <= this.targetRadius)
        {
            Debug.Log("Green State: Green Player " + this.playerID + " is Jailed!");
            //jailed = true;
            return true;
        }
        else
        {
            Debug.Log($"Green Player {this.playerID} is moving towards the jail! {direction.magnitude} and the target radias is {this.targetRadius}");
        }

        return false;
    }

    private void MoveAwayAndAvoidObstacles(Vector2 startingPoint, Vector2 endPoint)
    {
        Vector2 movementSum = Vector2.zero;
        Vector2 direction;
        float distance;
        float repulsionGeneral = 0.5f;
        float repulsionPurplePlayer = 2.0f;

        //Local scope For purple player
        direction = startingPoint - endPoint;
        //distance = direction.magnitude;

         movementSum -= repulsionPurplePlayer * direction ;
        

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

        //Move();
        MoveTo(this.Position(), movementSum);
        //return (distance <= currentSpeed * Time.deltaTime);
    }

    private bool MoveAway(Vector2 startingPoint, Vector2 endPoint)
    {
        //AvoidObstacles();
        float repulsion = 5.0f;

        Vector2 direction = startingPoint - endPoint;

        direction *= repulsion;
        
        float distance = direction.magnitude;

        
        float futureRotation = Mathf.Atan2(direction.y, direction.x);

        // Limit the rotation and linear speeds
        currentRotation += Mathf.Clamp(futureRotation - currentRotation, -maxRotationSpeed, maxRotationSpeed);
        

        return (distance <= currentSpeed * Time.deltaTime);
    }

    // The Move method that avoids obstacles
    public void CompositeMove()
    {
        base.Move(null, greenPlayers, obstacles);

    }

}
