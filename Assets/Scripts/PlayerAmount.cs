using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAmount : MonoBehaviour
{
    public GameObject InputManager;
    public GameObject m_PlayerPrefab;

    public bool backFromEnd = false;
    private GameObject playerVarsObj;
    public int playerAmount = 0;
    //public bool[] controllerInputs = new bool[4];
    //public int[] playerOrder = new int[4];
    public int[] charSelect = new int[4];
    public string[] charNames = new string[6];
    public bool[] playerIn = new bool[4];
    public string[] inputTimerType = new string[4];
    //public bool[] keyBoardIn = new bool[4];
    //public bool[] controllerIn = new bool[4];
    //public bool[] controllerJoinTest = new bool[4];
    public bool loadDone = false;
    public int scoreLimit = 10;
    public float timeLimitMin =1f;
    public float timeLimitMax = 5f;
    public float timeLimitDec = 0.3f;
    public string currentLevelName;
    public bool[] tipsRead = new bool[5];

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
        GameObject inputManager = GameObject.FindGameObjectWithTag("menuInput");
        if (activePlayers.Length != 0)
        {
            foreach (GameObject player in activePlayers)
            {
                player.GetComponent<playerInputList>().UpdateScenedata();
            }
        }
        if(inputManager != null)
        inputManager.GetComponent<MenuInputUserControls>().UpdateScenedata();
    }

    void Start()
    {


        playerVarsObj = GameObject.Find("PlayerVars");
        if (playerVarsObj != null)
        {
            backFromEnd = true;
            Debug.Log("Found PlayerVarsObj");
            playerVarsObj.GetComponent<PlayerVars>().playerIn.CopyTo(playerIn, 0);
            playerVarsObj.GetComponent<PlayerVars>().charSelect.CopyTo(charSelect, 0);
            playerVarsObj.GetComponent<PlayerVars>().tipsRead.CopyTo(tipsRead, 0);
            playerVarsObj.GetComponent<PlayerVars>().inputTimerType.CopyTo(inputTimerType, 0);
            //playerVarsObj.GetComponent<PlayerVars>().charNames.CopyTo(playerNames, 0);
        }
        else if (GameObject.FindGameObjectWithTag("menuInput") == null)
        {
            Debug.Log("Did not find PlayerVarsObj or existing inut manager, assume new game.");
            Instantiate(InputManager);
            PlayerInput.Instantiate(m_PlayerPrefab, playerIndex: 4, splitScreenIndex: -1,
                controlScheme: null, pairWithDevices: null);
        }
        loadDone = true;
    }

    void Update()
    {
        // if this is not the OpeningMenu or loading screen, destroy this object
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "OpeningMenu" && sceneName != "Loading") {
            Destroy(gameObject);
        }
    }
}
