using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    //This is the Respawn Manager script 
    //It finds all other current player's location and find random spawnpoint out of safe distance when reapawning a dead player at the beginning of their turn.

    //Safe Distance radius
    [SerializeField]
    private float SafeDistance = 25f;

    //References
    private TurnSwitch tScript;
    private GameObject[] playerObjects = new GameObject[4];
    //private int playerAmount = 0;
    [SerializeField]
    private List<Vector3> spawnPoints = new List<Vector3>();
    [SerializeField]
    private List<Vector3> selectedPoints = new List<Vector3>();

    private GameObject playerVarsObj;

    //Creating player position list for easier calculations
    //[SerializeField]
    private List<Vector3> playerPos = new List<Vector3>();

    //private int[] playerOrder;

    public bool[] playerIn = new bool[4];
    public bool playerInCapture = false;

    void Start()
    {
        //Reference to TurnSwitch script
        tScript = gameObject.GetComponent<TurnSwitch>();

        // Get player amount from PlayerVars
        playerVarsObj = GameObject.Find("PlayerVars");

        //Set player array & amount according to TurnScript
        playerObjects = tScript.playerObjects;
        //playerAmount = 4;//playerVarsObj.GetComponent<PlayerVars>().playerAmount;

        // get player order array from PlayerVars object
        //playerOrder = playerVarsObj.GetComponent<PlayerVars>().playerOrder;
        //playerVarsObj.GetComponent<PlayerVars>().playerIn.CopyTo(playerIn, 0);
    }

    ///<summary>
    ///Respawn function which selects a position from the map's spawnpoints and teleport the player there
    ///</summary>
    public void Respawn(int playerID)
    {
        //Debug.Log("Respawn function triggered" + Random.value);
        //Creating player position list for easier calculations
        List<Vector3> playerPos = new List<Vector3>();

        for (int i =1; i<=4; i++)
        {
            //Only add players that's still alive to player position list
            if (playerID != i && playerIn[i-1] && !playerObjects[i - 1].GetComponent<Player>().imDead)
            {
                playerPos.Add(playerObjects[i - 1].transform.position);
            }
        }

        //Calculate furthest spawnpoint available  (in case there are no spawnpoints in safe distance)
        Vector3 bestPoint = new Vector3(9999f, 9999f);
        float calculatedDistance = 9999f;
        float longestDistance = 0f;
        foreach (Vector3 point in spawnPoints)
        {
            calculatedDistance = 9999f;
            foreach (Vector3 player in playerPos)
            {
                calculatedDistance = Mathf.Min(calculatedDistance, Vector3.Distance(point, player));
            }
            if (calculatedDistance > longestDistance)
            {
                longestDistance = calculatedDistance;
                bestPoint = point;
            }
        }

        //find all points that's out of safe distance
        foreach (Vector3 point in spawnPoints)
        {
            //Debug.Log("Spawning player "+playerID+". Analyzing this point: x: "+point.x+" y: "+point.y);
            bool goodPoint = true;
            foreach (Vector3 player in playerPos)
            {
                //Debug.Log("Spawning player " + playerID + ". Point (x: " + point.x + " y: " + point.y + ")'s distance to player: " + Vector3.Distance(point, player));
                if(Vector3.Distance(point, player)< SafeDistance)
                {
                    goodPoint = false;
                    //Debug.Log("Spawning player " + playerID + ". Point (x: " + point.x + " y: " + point.y + ") is bad");
                    break;
                }
            }
            if (goodPoint)
            {
               //Debug.Log("Spawning player " + playerID + ". Point (x: " + point.x + " y: " + point.y + ") is good");
                selectedPoints.Add(point);
            }
        }

        //Randomly choose one point to respawn player
        if (!selectedPoints.Any())
        {
            Debug.Log("Spawning player " + playerID + ". Caution: There are no good spawn points available off safe distance. Spawning at furthest possible.");
            //Respawn the player at the best spawnpoint possible
            if (bestPoint.x == 9999f)
            {
                Debug.Log("Spawning player " + playerID + ". Error: bestPoint in Respawn() not captured.");
            }
            else
            {
                playerObjects[playerID - 1].transform.position = bestPoint;
            }
        }
        else
        {
            int randomIndex = Random.Range(0, selectedPoints.Count);
            Vector3 chosenRespawnPoint = selectedPoints[randomIndex];
            //Debug.Log("Spawning player " + playerID + ". Point selected to be respawn point: (x: " + chosenRespawnPoint.x + " y: " + chosenRespawnPoint.y);
            playerObjects[playerID - 1].transform.position = chosenRespawnPoint;
            selectedPoints.Clear();
        }
    }

    /* [LEGACY IMPORT METHOD]
    ///<summary>
    ///This should be called at the start of a game to set the player numbers and level-specific spawnpoints before calling Respawn()
    ///</summary>
    public void InitRespawn(int _playerAmount, List<Vector3> _spawnPoints)
    {
        playerAmount = _playerAmount;
        spawnPoints = _spawnPoints;
    }
    */

    void Update()
    {
        if (!playerInCapture)
        {
            playerVarsObj.GetComponent<PlayerVars>().playerIn.CopyTo(playerIn, 0);
            playerInCapture = true;
        }
        //Game logic script should specify how player death teleport/ability constraints works so this needs to be reworked
        // [Test] Change/remove after game logic handles player's death and turns movement restrictions
        /*
            if (Input.GetKeyDown(KeyCode.G))
                Respawn(1);
            else if (Input.GetKeyDown(KeyCode.H))
                Respawn(2);
        */
    }
}
