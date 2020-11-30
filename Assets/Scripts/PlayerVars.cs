using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerVars : MonoBehaviour
{
    private GameObject playerAmountObj;
    private GameObject cam;
    public int playerAmount = 4;
    //public bool[] controllerInputs = new bool[4];
    //public int[] playerOrder = { 0, 1, 2, 3 };
    public int[] charSelect = new int[4];
    public bool loadDone = false;
    public bool[] playerIn = new bool[4];
    public string[] inputTimerType = new string[4];
    //public bool[] keyBoardIn = new bool[4];
    //public bool[] controllerIn = new bool[4];
    //public bool[] controllerJoinTest = new bool[4];

    public int[] playerScores = new int[4];
    public string[] playerNames = new string[4];

    public int winningPlayerIndex = 0;
    public int winningCharSelectIndex = 0;
    public string winningCharName = "";
    public bool[] tipsRead = new bool[5];

    public int scoreLimit = 10;
    public float timeLimitMin = 1f;
    public float timeLimitMax = 5f;
    public float timeLimitDec = 0.3f;
    public string recentlyPlayedLevelName = "";

    void DontDestroy()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Awake()
    {
        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
        GameObject inputManager = GameObject.FindGameObjectWithTag("menuInput");
        if (activePlayers.Length != 0)
        {
            foreach (GameObject player in activePlayers)
            {
                player.GetComponent<playerInputList>().UpdateScenedata();
            }
        }
        inputManager.GetComponent<MenuInputUserControls>().UpdateScenedata();
    }

    void Start()
    {
        playerAmountObj = GameObject.Find("PlayerAmountObj");
        cam = GameObject.Find("Main Camera");

        // Get important player & char information from playerAmountObj if we can find it
        if (playerAmountObj != null) {
            Debug.Log("Found playerAmountObj");
            playerAmount = playerAmountObj.GetComponent<PlayerAmount>().playerAmount;
            //playerAmountObj.GetComponent<PlayerAmount>().controllerInputs.CopyTo(controllerInputs, 0);
            //playerAmountObj.GetComponent<PlayerAmount>().playerOrder.CopyTo(playerOrder, 0);
            playerAmountObj.GetComponent<PlayerAmount>().playerIn.CopyTo(playerIn, 0);
            playerAmountObj.GetComponent<PlayerAmount>().charSelect.CopyTo(charSelect, 0);
            playerAmountObj.GetComponent<PlayerAmount>().charNames.CopyTo(playerNames, 0);
            playerAmountObj.GetComponent<PlayerAmount>().tipsRead.CopyTo(tipsRead, 0);
            playerAmountObj.GetComponent<PlayerAmount>().inputTimerType.CopyTo(inputTimerType, 0);

            //playerAmountObj.GetComponent<PlayerAmount>().keyBoardIn.CopyTo(keyBoardIn, 0);
            //    playerAmountObj.GetComponent<PlayerAmount>().controllerIn.CopyTo(controllerIn, 0);
            //    playerAmountObj.GetComponent<PlayerAmount>().controllerJoinTest.CopyTo(controllerJoinTest, 0);

            scoreLimit = playerAmountObj.GetComponent<PlayerAmount>().scoreLimit;
            timeLimitMin = playerAmountObj.GetComponent<PlayerAmount>().timeLimitMin;
            timeLimitMax = playerAmountObj.GetComponent<PlayerAmount>().timeLimitMax;
            timeLimitDec = playerAmountObj.GetComponent<PlayerAmount>().timeLimitDec;
        }
        else {
            Debug.LogError("Did not find playerAmountObj, player controls won't work. Please Start the game from Opening Menu scene.");
            //playerOrder.CopyTo(charSelect, 0);
            // fill in default player names
            playerNames[0] = "BOXTER";
            playerNames[1] = "SNOOTS";
            playerNames[2] = "ROBOROO";
            playerNames[3] = "K-REX";
            //keyBoardIn[0] = true;
            //keyBoardIn[1] = true;
            //keyBoardIn[2] = true;
            //keyBoardIn[3] = true;
        }
        loadDone = true;

        // if we are in a level, get the score limit
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "TestLevel" || sceneName == "Carnival" || sceneName == "Industrial") {
            scoreLimit = cam.GetComponent<CheckForWinner>().scoreLimit;
            recentlyPlayedLevelName = sceneName;
        }

        // This object should last between scenes
        Invoke("DontDestroy", 0.1f);
    }

    void Update()
    {
        // get winningPlayerIndex and winningCharSelectIndex
        winningPlayerIndex = 0;
        winningCharSelectIndex = 0;
        for (int i = 0; i < 4; i++) {
            if (playerScores[i] > playerScores[winningPlayerIndex]) {
                winningPlayerIndex = i;
            }
        }
        winningCharSelectIndex = charSelect[winningPlayerIndex];
        winningCharName = playerNames[winningCharSelectIndex];

        // destroy self once in OpeningMenu, so we can create a new PlayerVars obj
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "OpeningMenu") {
            playerAmountObj = GameObject.Find("PlayerAmountObj");
            if (playerAmountObj.GetComponent<PlayerAmount>().loadDone)
            {
                Destroy(gameObject);
            }
        }
    }
}
