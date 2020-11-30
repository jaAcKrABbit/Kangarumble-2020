using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSwitch : MonoBehaviour
{
    public GameObject[] playerObjects = new GameObject[4];
    public float[] playerTimeLimit = new float[4];

    private int playerAmount = 2;
    public int currentPlayerTurn = 0;
    public int firstActivePlayer = 0;

    public bool firstTurn = true;
    public bool firstTurnStarted = false;
    public bool canStartTimer = true;
    public bool canRunTimer = false;

    public GameObject currentActivePlayer;

    public float timeLimitMin = 1f;
    public float timeLimitMax = 5f;
    public float timeLimitDecrement = 0.5f;

    public float currentPlayerTimeFull = 5f;
    public float currentPlayerTimeRemaining = 5f;

    public GameObject timerUIObj;
    public GameObject countdownObj;
    public GameObject turnIndicatorObj;

    private GameObject playerAmountObj;
    private GameObject playerVarsObj;
    private GameObject soundManagerObj;
    private GameObject cameraObj;
    private GameObject gameEndObj;

    public bool nextTurnProcess = false;
    public bool killingHappening = false;

    void Start()
    {
        playerAmountObj = GameObject.Find("PlayerAmountObj");
        timeLimitMin = playerAmountObj.GetComponent<PlayerAmount>().timeLimitMin;
        timeLimitMax = playerAmountObj.GetComponent<PlayerAmount>().timeLimitMax;
        timeLimitDecrement = playerAmountObj.GetComponent<PlayerAmount>().timeLimitDec;
        // set initial time limits for each player
        for (int i = 0; i < 4; i++) {
            playerTimeLimit[i] = timeLimitMax;
        }
        currentPlayerTimeFull = timeLimitMax;
        currentPlayerTimeRemaining = timeLimitMax;
        currentActivePlayer = playerObjects[currentPlayerTurn];

        playerVarsObj = GameObject.Find("PlayerVars");
        playerAmount = playerVarsObj.GetComponent<PlayerVars>().playerAmount;

        timerUIObj = GameObject.Find("TimerUI");
        countdownObj = GameObject.Find("321Rumble");
        turnIndicatorObj = GameObject.Find("TurnIndicatorSprite");
        soundManagerObj = GameObject.Find("SoundManager");
        cameraObj = GameObject.Find("Main Camera");
        gameEndObj = GameObject.Find("GameEnd");

    }

    void Update()
    {
        if (firstTurn) {
            FindFirstActivePlayer();
        }

        DeactivateUnusedPlayers();

        currentActivePlayer = playerObjects[currentPlayerTurn];

        bool gameEnd = gameEndObj.GetComponent<GameEnd>().gameEnd;
        bool caughtUpPipe = cameraObj.GetComponent<CameraFollow>().caughtUpPipe;
        canRunTimer = (canStartTimer && caughtUpPipe && !gameEnd);

        if (!countdownObj.active) {

            if (canRunTimer) {
                currentPlayerTimeRemaining -= Time.deltaTime;
            }
            
            if (!firstTurnStarted) {
                firstTurnStarted = true;
                // play turn switch sound
                soundManagerObj.GetComponent<SoundManager>().PlayTurnSwitchSound();
                // make turn indicator scale in
                float subspriteLenOut = turnIndicatorObj.GetComponent<TurnIndicatorNew>().subspriteLenOut;
                turnIndicatorObj.GetComponent<TurnIndicatorNew>().subspriteLen = subspriteLenOut;
            }
        }

        if (currentPlayerTimeRemaining <= 0 && !nextTurnProcess) {
            nextTurnProcess = true;

            currentActivePlayer.GetComponent<Player>().myTurn = false;

            firstTurn = false;

            currentPlayerTurn++;
            if (currentPlayerTurn >= playerAmount) {
                currentPlayerTurn = 0;
            }
            while (!playerVarsObj.GetComponent<PlayerVars>().playerIn[currentPlayerTurn]) {
                currentPlayerTurn++;
                if (currentPlayerTurn >= playerAmount) {
                    currentPlayerTurn = 0;
                }
            }
            StartCoroutine(WaitForKillAndStartNextTurn());
        }


        // Clamp time limits between max and min
        for (int i = 0; i < 4; i++) {
            playerTimeLimit[i] = Mathf.Clamp(playerTimeLimit[i], timeLimitMin, timeLimitMax);
        }

        if (playerObjects[currentPlayerTurn].GetComponent<Player>().imDead)
        {
            Debug.Log("Respawn BUG detected, re-respawning player");
            playerObjects[currentPlayerTurn].GetComponent<Player>().imDead = false;
            gameObject.GetComponent<RespawnManager>().Respawn(currentPlayerTurn + 1);
        }
    }
    //Wait for kill to complete to start the next turn without ignoring a dead player
    IEnumerator WaitForKillAndStartNextTurn()
    {
        //Debug.Log("WAITING FOR KILL " +killingHappening +" "+ Random.value);
        while (killingHappening)
        {
            //Debug.Log("kill is happening");
            yield return new WaitForSecondsRealtime(0.1f);
        }
        startNextTurn();
    }

    void startNextTurn()
    {
        //If the next player is dead then respawn when turn starts
        //Debug.Log("Starting Next turn, checking imDead bool"+ Random.value);
        if (playerObjects[currentPlayerTurn].GetComponent<Player>().imDead)
        {
            //Debug.Log("Check Passed, go ahead and respawn" + Random.value);
            playerObjects[currentPlayerTurn].GetComponent<Player>().imDead = false;
            gameObject.GetComponent<RespawnManager>().Respawn(currentPlayerTurn + 1);
        }
        //Debug.Log("Turn switched" + Random.value);

        currentPlayerTimeFull = playerTimeLimit[currentPlayerTurn];
        currentPlayerTimeRemaining = currentPlayerTimeFull;

        // shrink size of player turn text
        timerUIObj.GetComponent<TimerUI>().turnTextScaleDest = 0;

        // play turn switch sound
        soundManagerObj.GetComponent<SoundManager>().PlayTurnSwitchSound();

        // make turn indicator scale in
        float subspriteLenOut = turnIndicatorObj.GetComponent<TurnIndicatorNew>().subspriteLenOut;
        turnIndicatorObj.GetComponent<TurnIndicatorNew>().subspriteLen = subspriteLenOut;

        canStartTimer = false;
        Invoke("StartTimer", 0.2f);

        nextTurnProcess = false;
    }

    void DeactivateUnusedPlayers()
    {        
        for (var i = 0; i < playerVarsObj.GetComponent<PlayerVars>().playerIn.Length; i++) {
            if (!playerVarsObj.GetComponent<PlayerVars>().playerIn[i]) {
                playerObjects[i].SetActive(false);
            }
        }
    }

    void FindFirstActivePlayer()
    {
        // find player to go first
        firstActivePlayer = 0;
        while (!playerVarsObj.GetComponent<PlayerVars>().playerIn[firstActivePlayer] && firstActivePlayer < 3) {
            firstActivePlayer++;
        }
        currentPlayerTurn = firstActivePlayer;
    }

    void StartTimer()
    {
        canStartTimer = true;
    }
}
