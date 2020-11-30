using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EndScene : MonoBehaviour
{
    public GameObject playerWinTextObj;
    public Sprite[] backgroundSprites = new Sprite[3];
    private GameObject backgroundObj;
    private GameObject playerVarsObj;
    public GameObject[] debugVoteText = new GameObject[4];
    public bool[] ableToVote = new bool[4];
    public bool[] controllerInputs = new bool[4];

    public KeyCode[] joinKey = new KeyCode[4];
    public string[] joinKeyController = new string[4];

    public bool[] playerIn = new bool[4];
    [SerializeField]
    private int ready = 0;

    private GameObject transitionManager;

    void Awake()
    {
        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
        if (activePlayers.Length != 0)
        {
            foreach (GameObject player in activePlayers)
            {
                player.GetComponent<playerInputList>().UpdateScenedata();
            }
        }
    }

    void Start()
    {
        playerVarsObj = GameObject.Find("PlayerVars");
        backgroundObj = GameObject.Find("Background");

        // set text for winning player
        int winningCharSelectIndex = playerVarsObj.GetComponent<PlayerVars>().winningCharSelectIndex;
        string winningPlayerName = playerVarsObj.GetComponent<PlayerVars>().playerNames[winningCharSelectIndex];

        playerVarsObj.GetComponent<PlayerVars>().playerIn.CopyTo(playerIn, 0);
        playerWinTextObj.GetComponent<Text>().text = winningPlayerName + " WINS!";

        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");

        //Disable (display N/A) debug text and vote feature for not activated players 
        for (int i = 0; i < 4; i++)
        {
            ableToVote[i] = true;

            if (!playerIn[i])
            {
                debugVoteText[i].GetComponent<Text>().text = "N/A";
                debugVoteText[i].SetActive(false);
                ableToVote[i] = false;
            }
        }

        string levelName = playerVarsObj.GetComponent<PlayerVars>().recentlyPlayedLevelName;
        int bgIndex = 0;
        if (levelName == "TestLevel") {
            bgIndex = 0;
        }
        else if (levelName == "Industrial") {
            bgIndex = 1;
        }
        else if (levelName == "Carnival") {
            bgIndex = 2;
        }
        backgroundObj.GetComponent<SpriteRenderer>().sprite = backgroundSprites[bgIndex];

        transitionManager = GameObject.Find("SceneTransition");
    }

    void Update()
    {
        //Vote system in callbacks

        if(ready >= playerVarsObj.GetComponent<PlayerVars>().playerAmount)
        {
            Debug.Log("All players voted, loading OpeningMenu");
            transitionManager.GetComponent<TransitionManagerMenu>().EndSceneClose();
        }

        playerWinTextObj.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, (Mathf.Sin(Time.time * 6) * 2f));
    }

    public void Vote(GameObject player)
    {
        int targetIndex = player.GetComponent<PlayerInput>().playerIndex;
        if (ableToVote[targetIndex])
        {
            debugVoteText[targetIndex].GetComponent<Text>().text = "READY!";
            ableToVote[targetIndex] = false;
            ready++;
            player.GetComponent<playerInputList>().Vibration();
        }
    }
}
