using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The GameManager keeps track of global information for the game, such as the players and the location of the gaol.
// The GameManager is a Singleton (see Nystrom, chapter II.6), there is only single instance of it.

public class GameManager : MonoBehaviour
{
    // This is the single instance of the class
    private static GameManager instance = null;

    // Keep track of all the players
    private const int numGreenPlayers = 5;
    private List<Player> greenPlayers = new List<Player>(numGreenPlayers);

    private const int numPurplePlayers = 3;
    private List<Player> purplePlayers = new List<Player>(numPurplePlayers);

    [SerializeField]
    GameObject greenPlayerPrefab;
    [SerializeField]
    GameObject purplePlayerPrefab;
    [SerializeField]
    GameObject gaol;
    //[SerializeField]
    //GameObject ground;
    
    [SerializeField]
    GameObject northWall;
    [SerializeField]
    GameObject southWall;
    [SerializeField]
    GameObject eastWall;
    [SerializeField]
    GameObject westWall;

    // A string to identify objects tagged as obstacles
    const string fenceTag = "Boundary";

    // A string to identify objects tagged as obstacles
    const string ObstacleTag = "Obstacle";

    List<GameObject> fences;
    //Keep track of captured green players within the game

    // We store all obstacles
    List<GameObject> obstacles;

    [SerializeField]
    GameObject gameOverObject;
    [SerializeField]
    Text gameOverText;
    [SerializeField]
    Text greenCapturedText;
    [SerializeField]
    Text timerText;

    //Reading the value from the inspector
    public float timer;

    private int countCapturedGreenPlayers = 0;
    private bool gameOver = false;
    private bool timeOut = false; //If the timer finishes
    private bool purpleWin = false; //if it is game over and purple didn't win meaning green won (To be implemented)

    protected float obstacleAvoidanceDistance = 0.50f;

    // Start is called before the first frame update
    void Start()
    {
        timeOut = false;
        gameOverObject.SetActive(false);
        // If there already is an official instance, this instance deletes itself.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        // Create all the players.
        for (int i = 0; i < numGreenPlayers; i++)
        {
            GameObject newGreenPlayer = Instantiate(greenPlayerPrefab, new Vector3(-15 + i * 5, 0, 15), Quaternion.identity);
            greenPlayers.Add(newGreenPlayer.GetComponent<GreenPlayer>());

            greenPlayers[i].playerID = i + 1;
            Debug.Log("Green Player " + greenPlayers[i].playerID + " at position " + greenPlayers[i].transform.position.x + " , " + greenPlayers[i].transform.position.z);
        }

        for (int i = 0; i < numPurplePlayers; i++)
        {
            GameObject newPurplePlayer = Instantiate(purplePlayerPrefab, new Vector3(-15 + i * 5, 0, -15), Quaternion.identity);
            purplePlayers.Add(newPurplePlayer.GetComponent<PurplePlayer>());
            purplePlayers[i].playerID = i + 1;

            Debug.Log("Purple Player " + purplePlayers[i].playerID + " at position " + purplePlayers[i].transform.position.x + " , " + purplePlayers[i].transform.position.z);
        }

        // Find the walls
        fences = new List<GameObject>(GameObject.FindGameObjectsWithTag(fenceTag));

        // Find the obstacles
        obstacles = new List<GameObject>(GameObject.FindGameObjectsWithTag(ObstacleTag));

        //horizontalBoundary = eastWall.transform.position.x - westWall.transform.position.x;
        //verticalBoundary = northWall.transform.position.z - southWall.transform.position.z;
        //Debug.Log($"East wall X is {eastWall.transform.position.x} and West wall X is {westWall.transform.position.x}");

        //Debug.Log($"North wall X is {northWall.transform.position.z} and South wall X is {southWall.transform.position.z}");

        //Debug.Log($"horizontal Boundary is {horizontalBoundary} and the vertical boundary is {verticalBoundary}");
    }

    private void Update()
    {
        if (!IsTimeOut())
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timeOut = true;
            }
            DisplayTime(timer);
        }
        

        if (IsGameOver() || IsTimeOut())
        {
            gameOverObject.SetActive(true);
            if (purpleWin)
            {
                gameOverText.text = "Purple Team Win";
                Debug.Log("Purple Team Win");
            }
            else
            {
                gameOverText.text = "Green Team Win";
                Debug.Log("Green Team Win");
                DisplayTime(0);
            }
            Time.timeScale = 0;
        }
    }

    public bool IsTimeOut()
    {
        return timeOut;
    }
    void DisplayTime(float timefordisplay)
    {
        float minute = Mathf.FloorToInt(timefordisplay / 60);
        float second = Mathf.FloorToInt(timefordisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minute, second);

    }
    // Find the nearest green player to a given purple player
    public GreenPlayer FindClosestTarget(PurplePlayer player)
    {
        GreenPlayer target = null;
        //float closestDistance = float.MaxValue;
        float closestDistance = 5;

        foreach (GreenPlayer greenPlayer in greenPlayers)
        {
            float distance = Vector2.Distance(greenPlayer.Position(), player.Position());

            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = greenPlayer;
            }
        }
        return target;
    }

    // Find the nearest purple player to a given green player

    public PurplePlayer FindClosestTarget(GreenPlayer player)
    {
        PurplePlayer target = null;
        float closestDistance = float.MaxValue;

        foreach (PurplePlayer purplePlayer in purplePlayers)
        {
            float distance = Vector2.Distance(purplePlayer.Position(), player.Position());

            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = purplePlayer;
            }
        }
        return target;
    }

    // Return the gaol object
    public GameObject GetGaol()
    {
        return gaol;
    }

    // Return the single instance of the class
    public static GameManager Instance()
    {
        return instance;
    }

    public void GreenPlayerCaptured()
    {
        countCapturedGreenPlayers++;
        greenCapturedText.text = $"Captured Green Players: {countCapturedGreenPlayers}"; 
        Debug.Log("A Green Players has been captured! Current Count " + countCapturedGreenPlayers);
    }

    public List<Player> GreenPlayers()
    {
        return greenPlayers;
    }

    public List<Player> PurplePlayers()
    {
        return purplePlayers;
    }
    
    public List<GameObject> Fences()
    {
        return fences;
    }

    public List<GameObject> Obstacles()
    {
        return obstacles;
    }

    public void DisplayPosition(string name, Player player)
    {
        Debug.Log(name + " " + player.playerID + ": " + player.transform.position.x + " , " + player.transform.position.z);
    }

    public bool CheckPlayerDistanceWithinBoundaries(Vector2 playerPosition)
    {
        if (Mathf.Abs(playerPosition.x - eastWall.transform.position.x) < obstacleAvoidanceDistance)
        {
            Debug.Log("Player is near the east wall!!");
            return false;
        }
        if (Mathf.Abs(playerPosition.x - westWall.transform.position.x) < obstacleAvoidanceDistance)
        {
            Debug.Log("Player is near the west wall!!");
            return false;
        }
        if (Mathf.Abs(playerPosition.y - northWall.transform.position.z) < obstacleAvoidanceDistance)
        {
            Debug.Log("Player is near the north wall!!");
            return false;
        }

        if (Mathf.Abs(playerPosition.y - southWall.transform.position.z) < obstacleAvoidanceDistance)
        {
            Debug.Log("Player is near the south wall!!");
            return false;
        }

        foreach (GameObject obstacle in obstacles)
        {
            Vector2 obstaclePosition = new Vector2(obstacle.transform.position.x, obstacle.transform.position.z);
            Vector2 direction = playerPosition - obstaclePosition;
            float distance = direction.magnitude;

            if (distance < 0.25)
            {
                Debug.Log("Player is near an obstacle!!");
                return false;
            }
        }
        return true;
    }
    public bool IsGameOver()
    {
        if (countCapturedGreenPlayers == numGreenPlayers)
        {
            Debug.Log("Game Over Purple Player Won");
            purpleWin = true;
            gameOver = true;
        }

        return gameOver;
    }

    public void RemoveGreenPlayer(GreenPlayer target)
    {
        //Remove the player from the GreenPlayers reference list
        greenPlayers.Remove(target);
    }
}
