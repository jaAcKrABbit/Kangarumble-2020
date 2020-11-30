using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerSelect : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    private GameObject playerAmountObj;
    private GameObject levelSelectObj;
    private bool inputAdvance;
    public bool inputAccept = false;

    public bool[] playerIn = new bool[4];
    public int playersIn = 0;
    public int playersLockedIn = 0;
    public bool ableToContinue = false;

    public GameObject[] kangaObj = new GameObject[6];

    public GameObject joinTheGameParentObj;
    public GameObject joinTheGameObj;
    public GameObject joinControllerObj;
    public GameObject joinKeyboardObj;

    public GameObject continueParentObj;
    public GameObject continueObj;
    public GameObject continueControllerObj;
    public GameObject continueKeyboardObj;

    //New input system readValues
    public string[] inputTimerType = new string[4];
    public bool[] leftInput = new bool[4];
    public bool[] rightInput = new bool[4];

    public float kangaY;
    public float kangaZ;

    public string loadLevel;

    public GameObject[] charSelectArrow = new GameObject[4];
    public GameObject[] charLockedInSprite = new GameObject[4];
    public float charLockedInSpriteY = 0;
    public float[] charSelectArrowX = new float[4];
    public float[] charSelectArrowXDest = new float[4];
    public float[] charSelectArrowY = new float[4];
    public float[] charSelectArrowYDest = new float[4];
    public float charSelectArrowYPlayerNotIn = -400;
    public float charSelectArrowYNotLockedIn = 100;
    public float charSelectArrowYLockedIn = 200;
    public int[] charSelect = new int[4];
    public bool[] charSelectLockedIn = new bool[4];

    public bool allPlayersLockedIn = false;

    public bool[] inputTimerKeyboard = new bool[4];
    public bool[] inputTimerController = new bool[4];
    public float inputDelayTimeKeyboard;
    public float inputDelayTimeController;

    public float arrowBobber = 0;

    public GameObject[] charNameObj = new GameObject[6];
    public float charNameY;
    private float charNamesScale = 1;
    private float charNamesScaleDest = 1;

    public List<int> playersOnChar0 = new List<int>();
    public List<int> playersOnChar1 = new List<int>();
    public List<int> playersOnChar2 = new List<int>();
    public List<int> playersOnChar3 = new List<int>();
    public List<int> playersOnChar4 = new List<int>();
    public List<int> playersOnChar5 = new List<int>();

    public bool endLoaded = false;

    public OpeningMenuSoundManager soundmanager;

    public float smoothDampRate = 14f;
    public float smoothDampRateArrows = 20f;
    public float smoothDampRateText = 11f;

    void Start()
    {
        // whatever level should be loaded
        loadLevel = "TestLevel";

        playerAmountObj = GameObject.Find("PlayerAmountObj");
        levelSelectObj = GameObject.Find("LevelSelect");
        playerInputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<PlayerInputManager>();

        for (var i = 0; i < 4; i++) {
            charSelectLockedIn[i] = false;
            leftInput[i] = false;
            rightInput[i] = false;
        }
    }

    void Update()
    {
        if (!endLoaded && playerAmountObj.GetComponent<PlayerAmount>().loadDone) {
            //reload selection from end scene if so
            if (playerAmountObj.GetComponent<PlayerAmount>().backFromEnd)
            {
                gameObject.GetComponent<OpeningMenu>().menuState = 2;
                playerAmountObj.GetComponent<PlayerAmount>().playerIn.CopyTo(playerIn, 0);
                playerAmountObj.GetComponent<PlayerAmount>().inputTimerType.CopyTo(inputTimerType, 0);
                for (var i = 0; i < 4; i++)
                {
                    if (playerIn[i])
                    {
                        playerIn[i] = true;
                        charSelect[i] = playerAmountObj.GetComponent<PlayerAmount>().charSelect[i];
                        charSelectLockedIn[i] = true;
                    }
                }
            }
            endLoaded = true;
        }
        if (gameObject.GetComponent<OpeningMenu>().menuState == 0
        || gameObject.GetComponent<OpeningMenu>().menuState == 1)
        {
            GameObject[] activePlayers;
            activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
            foreach (GameObject player in activePlayers)
            {
                Destroy(player);
            }
        }

        //New joining keyboards using new Input System
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2 && Keyboard.current.sKey.wasPressedThisFrame)
        {
            GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
            int playerindex = 0;

            if (activePlayers.Length != 0)
            {
                for(int i = 0; i < 4; i++)
                {
                    bool goodIndex = true;
                    foreach (GameObject player in activePlayers)
                    {
                        if(player.GetComponent<PlayerInput>().playerIndex == i)
                        {
                            goodIndex = false;
                        }
                    }
                    if (goodIndex)
                    {
                        playerindex = i;
                        break;
                    }
                }
            }

            PlayerInput temp = playerInputManager.JoinPlayer(playerindex, -1, "Keyboard0", Keyboard.current);
            if (temp != null)
            {
                Debug.Log($"Keyboard player successfully joined with [Keyboard 0] Scheme and index as {temp.playerIndex}");
                playerIn[temp.playerIndex] = true;
                inputTimerType[temp.playerIndex] = "keyboard";
                inputTimerKeyboard[temp.playerIndex] = false;
                InvokeInputTimerKeyboard(temp.playerIndex);
            }
        }
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2 && Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
            int playerindex = 0;

            if (activePlayers.Length != 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    bool goodIndex = true;
                    foreach (GameObject player in activePlayers)
                    {
                        if (player.GetComponent<PlayerInput>().playerIndex == i)
                        {
                            goodIndex = false;
                        }
                    }
                    if (goodIndex)
                    {
                        playerindex = i;
                        break;
                    }
                }
            }

            PlayerInput temp = playerInputManager.JoinPlayer(playerindex, -1, "Keyboard1", Keyboard.current);
            if (temp != null)
            {
                Debug.Log($"Keyboard player successfully joined with [Keyboard 1] Scheme and index as {temp.playerIndex}");
                playerIn[temp.playerIndex] = true;
                inputTimerType[temp.playerIndex] = "keyboard";
                inputTimerKeyboard[temp.playerIndex] = false;
                InvokeInputTimerKeyboard(temp.playerIndex);
            }
        }
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2 && Keyboard.current.gKey.wasPressedThisFrame)
        {
            GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
            int playerindex = 0;

            if (activePlayers.Length != 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    bool goodIndex = true;
                    foreach (GameObject player in activePlayers)
                    {
                        if (player.GetComponent<PlayerInput>().playerIndex == i)
                        {
                            goodIndex = false;
                        }
                    }
                    if (goodIndex)
                    {
                        playerindex = i;
                        break;
                    }
                }
            }

            PlayerInput temp = playerInputManager.JoinPlayer(playerindex, -1, "Keyboard2", Keyboard.current);
            if (temp != null)
            {
                Debug.Log($"Keyboard player successfully joined with [Keyboard 1] Scheme and index as {temp.playerIndex}");
                playerIn[temp.playerIndex] = true;
                inputTimerType[temp.playerIndex] = "keyboard";
                inputTimerKeyboard[temp.playerIndex] = false;
                InvokeInputTimerKeyboard(temp.playerIndex);
            }
        }
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2 && Keyboard.current.kKey.wasPressedThisFrame)
        {
            GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
            int playerindex = 0;

            if (activePlayers.Length != 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    bool goodIndex = true;
                    foreach (GameObject player in activePlayers)
                    {
                        if (player.GetComponent<PlayerInput>().playerIndex == i)
                        {
                            goodIndex = false;
                        }
                    }
                    if (goodIndex)
                    {
                        playerindex = i;
                        break;
                    }
                }
            }

            PlayerInput temp = playerInputManager.JoinPlayer(playerindex, -1, "Keyboard3", Keyboard.current);
            if (temp != null)
            {
                Debug.Log($"Keyboard player successfully joined with [Keyboard 1] Scheme and index as {temp.playerIndex}");
                playerIn[temp.playerIndex] = true;
                inputTimerType[temp.playerIndex] = "keyboard";
                inputTimerKeyboard[temp.playerIndex] = false;
                InvokeInputTimerKeyboard(temp.playerIndex);
            }
        }

        //New Left/Right selection based on inputs with timer

        for(int i = 0; i < 4; i++)
        {
            if (!charSelectLockedIn[i])
            {
                if (leftInput[i])
                {
                    if (inputTimerType[i] == "keyboard" && inputTimerKeyboard[i])
                    {
                        charSelect[i]--;
                        if (charSelect[i] < 0)
                        {
                            charSelect[i] = 5;
                        }
                        inputTimerKeyboard[i] = false;
                        InvokeInputTimerKeyboard(i);
                    }
                    else if (inputTimerType[i] == "controller" && inputTimerController[i])
                    {
                        charSelect[i]--;
                        if (charSelect[i] < 0)
                        {
                            charSelect[i] = 5;
                        }
                        inputTimerController[i] = false;
                        InvokeInputTimerController(i);
                    }
                }
                else if (rightInput[i])
                {
                    if (inputTimerType[i] == "keyboard" && inputTimerKeyboard[i])
                    {
                        charSelect[i]++;
                        if (charSelect[i] > 5)
                        {
                            charSelect[i] = 0;
                        }
                        inputTimerKeyboard[i] = false;
                        InvokeInputTimerKeyboard(i);
                    }
                    else if (inputTimerType[i] == "controller" && inputTimerController[i])
                    {
                        charSelect[i]++;
                        if (charSelect[i] > 5)
                        {
                            charSelect[i] = 0;
                        }
                        inputTimerController[i] = false;
                        InvokeInputTimerController(i);
                    }
                }
            }
               
        }
        //New Dropping/Returning keyboards with new input system (legacy)

        ableToContinue = (playersIn >= 2 && playersIn <= 4 && playersLockedIn >= 2 && playersIn == playersLockedIn);
        pressObjsNew();
        if (gameObject.GetComponent<OpeningMenu>().menuState == 0
        || gameObject.GetComponent<OpeningMenu>().menuState == 1) {
            inputAccept = false;

            // reset variables if we return to very first openingScreen
            for (var i = 0; i < 4; i++) {
                playerIn[i] = false;
                leftInput[i] = false;
                rightInput[i] = false;
                charSelectLockedIn[i] = false;
                charSelect[i] = i;
            }
        }

        if (gameObject.GetComponent<OpeningMenu>().menuState == 2) {
            inputAccept = true;
        }
        if (gameObject.GetComponent<OpeningMenu>().menuState == 3) {
            for (var i = 0; i < 4; i++)
            {
                leftInput[i] = false;
                rightInput[i] = false;
            }
        }

        // refresh playersIn and playersLockedIn
        playersIn = 0;
        playersLockedIn = 0;
        for (int i = 0; i < 4; i++) {
            playersIn += (playerIn[i]) ? 1 : 0;
            playersLockedIn += (charSelectLockedIn[i]) ? 1 : 0;
        }

        kangaY = Screen.height * 0.24f;
        charLockedInSpriteY = Screen.height * 0.18f;
        charNameY = Screen.height * 0.1f;

        // players join and drop now at Bottom callbacks
        for (int i = 0; i < 6; i++) {
            // set x position for kangaroo sprites
            GameObject currentKangaObj = kangaObj[i];
            //currentKangaObj.SetActive(playerIn[i]);
            float currentKangaX = 0;
            currentKangaX = (Screen.width / 7) * (i + 1);
            currentKangaObj.GetComponent<Transform>().position = new Vector3(currentKangaX, kangaY, kangaZ);
        }
        // go to level moved to callbacks
        CharSelect();

    }


    void CharSelect()
    {
        // refresh playersOnChar lists
        playersOnChar0.Clear();
        playersOnChar1.Clear();
        playersOnChar2.Clear();
        playersOnChar3.Clear();
        playersOnChar4.Clear();
        playersOnChar5.Clear();
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2)
        {
            for (var i = 0; i < 4; i++)
            {
                if (playerIn[i] && !charSelectLockedIn[i])
                {
                    if (charSelect[i] == 0)
                    {
                        playersOnChar0.Add(i);
                    }
                    else if (charSelect[i] == 1)
                    {
                        playersOnChar1.Add(i);
                    }
                    else if (charSelect[i] == 2)
                    {
                        playersOnChar2.Add(i);
                    }
                    else if (charSelect[i] == 3)
                    {
                        playersOnChar3.Add(i);
                    }
                    else if (charSelect[i] == 4)
                    {
                        playersOnChar4.Add(i);
                    }
                    else if (charSelect[i] == 5)
                    {
                        playersOnChar5.Add(i);
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                //Lock in moved to new callbacks
                // set x destination of char select arrow
                charSelectArrowXDest[i] = kangaObj[charSelect[i]].transform.position.x;
            }
        }


        AdjustCharSelectArrows();

        charSelectArrowYPlayerNotIn = Screen.height + 70;
        charSelectArrowYNotLockedIn = Screen.height * 0.7f;
        charSelectArrowYLockedIn = Screen.height * 0.5f;

        for (int i = 0; i < 4; i++) {
            GameObject currentCharSelectArrow = charSelectArrow[i];

            // set arrow visbible if player is in
            if (gameObject.GetComponent<OpeningMenu>().menuState == 2) {
                if (playerIn[i]) {
                    charSelectArrowYDest[i] = (charSelectLockedIn[i]) ? charSelectArrowYLockedIn : charSelectArrowYNotLockedIn;
                }
                else {
                    charSelectArrowYDest[i] = charSelectArrowYPlayerNotIn;
                }
            }
            else {
                charSelectArrowYDest[i] = charSelectArrowYPlayerNotIn;
            }
            currentCharSelectArrow.SetActive(gameObject.GetComponent<OpeningMenu>().menuState == 2);

            float currentTimeRelative = Time.time + (i * 250);
            arrowBobber = (Mathf.Sin(currentTimeRelative * 8) * 6f);

            // set XY position of charSelectArrow
            charSelectArrowX[i] = Damp(charSelectArrowX[i], charSelectArrowXDest[i], smoothDampRateArrows, Time.deltaTime);
            charSelectArrowY[i] = Damp(charSelectArrowY[i], charSelectArrowYDest[i], smoothDampRateArrows, Time.deltaTime);

            currentCharSelectArrow.GetComponent<Transform>().position = new Vector3(
                charSelectArrowX[i],
                charSelectArrowY[i] + arrowBobber,
                currentCharSelectArrow.transform.position.z
            );

            // set position of lockedIn sprite
            GameObject currentCharLockedInSprite = charLockedInSprite[i];
            currentCharLockedInSprite.SetActive(charSelectLockedIn[i]);
            currentCharLockedInSprite.GetComponent<Transform>().position = new Vector3(
                kangaObj[charSelect[i]].transform.position.x,
                charLockedInSpriteY,
                currentCharLockedInSprite.transform.position.z
            );
            currentCharLockedInSprite.transform.Rotate(0, 0, 50 * Time.deltaTime);
        }

        // set positions, scale, and strings for character names
        charNamesScaleDest = (gameObject.GetComponent<OpeningMenu>().menuState >= 2) ? 1 : 0;
        charNamesScale = Damp(charNamesScale, charNamesScaleDest, smoothDampRate, Time.deltaTime);

        Vector3 charNamesScaleVec = new Vector3(charNamesScale, charNamesScale, 1);
        for (int i = 0; i < 6; i++) {
            string currentCharName = playerAmountObj.GetComponent<PlayerAmount>().charNames[i];
            charNameObj[i].GetComponent<Transform>().position = new Vector3(
                kangaObj[i].transform.position.x,
                charNameY,
                1
            );
            charNameObj[i].GetComponent<Transform>().localScale = charNamesScaleVec;
            charNameObj[i].GetComponent<Text>().text = currentCharName;
        }
    }

    void AdjustCharSelectArrows() {

        // adjust x-position of charSelectArrows if >1 arrow are on same position
        int arrowAdjustSpacing = 15;
        for (var i = 0; i < 4; i++) {
            List<int> currentList;
            if (i == 0) {
                currentList = playersOnChar0;
            }
            else if (i == 1) {
                currentList = playersOnChar1;
            }
            else if (i == 2) {
                currentList = playersOnChar2;
            }
            else {
                currentList = playersOnChar3;
            }
            
            if (currentList.Count == 2) {
                charSelectArrowXDest[currentList[0]] -= arrowAdjustSpacing;
                charSelectArrowXDest[currentList[1]] += arrowAdjustSpacing;
            }
            else if (currentList.Count == 3) {
                charSelectArrowXDest[currentList[0]] -= arrowAdjustSpacing * 2;
                charSelectArrowXDest[currentList[2]] += arrowAdjustSpacing * 2;
            }
            else if (currentList.Count == 4) {
                charSelectArrowXDest[currentList[0]] -= arrowAdjustSpacing * 3;
                charSelectArrowXDest[currentList[1]] -= arrowAdjustSpacing;
                charSelectArrowXDest[currentList[2]] += arrowAdjustSpacing;
                charSelectArrowXDest[currentList[3]] += arrowAdjustSpacing * 3;
            }
        }
    }

    public void pressObjsNew()
    {
        float pressObjY = Screen.height * 0.82f;

        // JOIN IN!
        float joinTheGameParentScale = joinTheGameParentObj.GetComponent<RectTransform>().localScale.x;
        float joinTheGameParentScaleDest = 0;
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2 && !ableToContinue) {
            joinTheGameParentScaleDest = 0.25f;
        }
        joinTheGameParentScale = Damp(joinTheGameParentScale, joinTheGameParentScaleDest, smoothDampRate, Time.deltaTime);
        float currentTime = gameObject.GetComponent<OpeningMenu>().currentTime;
        float joinTheGameScale = (Mathf.Sin(currentTime * 4) / 10) + 1.1f;
        joinTheGameObj.GetComponent<RectTransform>().localScale = new Vector3(joinTheGameScale, joinTheGameScale, 1);
        joinTheGameObj.SetActive(joinTheGameParentScale > 0.05f);
        joinTheGameParentObj.GetComponent<RectTransform>().localScale = new Vector3(joinTheGameParentScale, joinTheGameParentScale, 1);
        joinTheGameObj.GetComponent<RectTransform>().position = new Vector3(Screen.width * 0.5f, pressObjY, 0);


        // Contoller press A, Keyboard press Down
        float joinControllerX = joinControllerObj.GetComponent<RectTransform>().position.x;
        float joinControllerXDest = -(Screen.width * 0.15f);
        float joinKeyboardX = joinKeyboardObj.GetComponent<RectTransform>().position.x;
        float joinKeyboardXDest = (Screen.width * 1.15f);
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2 && !ableToContinue) {
            joinControllerXDest = Screen.width * 0.15f;
            joinKeyboardXDest = Screen.width * 0.85f;
        }
        joinControllerX = Damp(joinControllerX, joinControllerXDest, smoothDampRate, Time.deltaTime);
        joinControllerObj.SetActive(Mathf.Abs(joinControllerX - (-(Screen.width * 0.15f))) > 2);
        joinControllerObj.GetComponent<RectTransform>().position = new Vector3(joinControllerX, pressObjY, 0);
        joinKeyboardX = Damp(joinKeyboardX, joinKeyboardXDest, smoothDampRate, Time.deltaTime);
        joinKeyboardObj.SetActive(Mathf.Abs(joinKeyboardX - (Screen.width * 1.15f)) > 2);
        joinKeyboardObj.GetComponent<RectTransform>().position = new Vector3(joinKeyboardX, pressObjY, 0);


        // CONTINUE?
        float continueParentScale = continueParentObj.GetComponent<RectTransform>().localScale.x;
        float continueParentScaleDest = 0;
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2 && ableToContinue) {
            continueParentScaleDest = 0.25f;
        }
        continueParentScale = Damp(continueParentScale, continueParentScaleDest, smoothDampRate, Time.deltaTime);
        continueObj.GetComponent<RectTransform>().localScale = new Vector3(joinTheGameScale, joinTheGameScale, 1);
        continueObj.SetActive(continueParentScale > 0.05f);
        continueParentObj.GetComponent<RectTransform>().localScale = new Vector3(continueParentScale, continueParentScale, 1);
        continueParentObj.GetComponent<RectTransform>().position = new Vector3(Screen.width * 0.5f, pressObjY, 0);


        // Controller press A to continue, Keyboard press ENTER to continue
        float continueControllerX = continueControllerObj.GetComponent<RectTransform>().position.x;
        float continueControllerXDest = -(Screen.width * 0.15f);
        float continueKeyboardX = continueKeyboardObj.GetComponent<RectTransform>().position.x;
        float continueKeyboardXDest = (Screen.width * 1.15f);
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2 && ableToContinue) {
            continueControllerXDest = Screen.width * 0.15f;
            continueKeyboardXDest = Screen.width * 0.85f;
        }
        continueControllerX = Damp(continueControllerX, continueControllerXDest, smoothDampRate, Time.deltaTime);
        continueControllerObj.SetActive(Mathf.Abs(continueControllerX - (-(Screen.width * 0.15f))) > 2);
        continueControllerObj.GetComponent<RectTransform>().position = new Vector3(continueControllerX, pressObjY, 0);
        continueKeyboardX = Damp(continueKeyboardX, continueKeyboardXDest, smoothDampRate, Time.deltaTime);
        continueKeyboardObj.SetActive(Mathf.Abs(continueKeyboardX - (Screen.width * 1.15f)) > 2);
        continueKeyboardObj.GetComponent<RectTransform>().position = new Vector3(continueKeyboardX, pressObjY, 0);
    }

    public void InvokeInputTimerKeyboard(int i)
    {
        string funcName = "";
        if (i == 0) {
            funcName = "ResetInputTimerKeyboard1";
        }
        else if (i == 1) {
            funcName = "ResetInputTimerKeyboard2";
        }
        else if (i == 2) {
            funcName = "ResetInputTimerKeyboard3";
        }
        else {
            funcName = "ResetInputTimerKeyboard4";
        }
        Invoke(funcName, inputDelayTimeKeyboard);
    }
    public void InvokeInputTimerController(int i)
    {
        string funcName = "";
        if (i == 0) {
            funcName = "ResetInputTimerController1";
        }
        else if (i == 1) {
            funcName = "ResetInputTimerController2";
        }
        else if (i == 2) {
            funcName = "ResetInputTimerController3";
        }
        else {
            funcName = "ResetInputTimerController4";
        }
        Invoke(funcName, inputDelayTimeController);
    }

    public void ResetInputTimerKeyboard1()
    {
        inputTimerKeyboard[0] = true;
    }
    public void ResetInputTimerKeyboard2()
    {
        inputTimerKeyboard[1] = true;
    }
    public void ResetInputTimerKeyboard3()
    {
        inputTimerKeyboard[2] = true;
    }
    public void ResetInputTimerKeyboard4()
    {
        inputTimerKeyboard[3] = true;
    }

    public void ResetInputTimerController1()
    {
        inputTimerController[0] = true;
    }
    public void ResetInputTimerController2()
    {
        inputTimerController[1] = true;
    }
    public void ResetInputTimerController3()
    {
        inputTimerController[2] = true;
    }
    public void ResetInputTimerController4()
    {
        inputTimerController[3] = true;
    }


    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }


    IEnumerator StopVibration(Gamepad gamepad, float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        gamepad.SetMotorSpeeds(0f, 0f);
    }

    //New input system joining action for gamepads
    public void JoinPlayer(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            
            InputDevice tempdevice = ctx.control.device;
            //Debug.Log(tempdevice);
            if (tempdevice.name !="Keyboard" && gameObject.GetComponent<OpeningMenu>().menuState == 2)
            {
                GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("playerInput");
                int playerindex = 0;

                if (activePlayers.Length != 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        bool goodIndex = true;
                        foreach (GameObject player in activePlayers)
                        {
                            if (player.GetComponent<PlayerInput>().playerIndex == i)
                            {
                                goodIndex = false;
                            }
                        }
                        if (goodIndex)
                        {
                            playerindex = i;
                            break;
                        }
                    }
                }
                PlayerInput temp = playerInputManager.JoinPlayer(playerindex, -1, null, tempdevice);
                if (temp != null)
                {
                    Debug.Log($"Gamepad player successfully joined with [{temp.devices[0].displayName}] device and index as {temp.playerIndex}");
                    playerIn[temp.playerIndex] = true;
                    inputTimerType[temp.playerIndex] = "controller";
                    inputTimerController[temp.playerIndex] = false;
                    InvokeInputTimerController(temp.playerIndex);
                }

            }
        }
    }

    //New input system drop/return
    public void Drop_Return(GameObject player)
    {
        if(gameObject.GetComponent<OpeningMenu>().menuState == 2)
        {
            //Determine if the player should be returned to selection or dropped
            int targetIndex = player.GetComponent<PlayerInput>().playerIndex;

            if (!charSelectLockedIn[targetIndex])
            {
                Debug.Log($"Dropped player {player.GetComponent<PlayerInput>().playerIndex} with device [{player.GetComponent<PlayerInput>().devices[0].displayName}] and scheme [{player.GetComponent<PlayerInput>().currentControlScheme}]");
                playerIn[targetIndex] = false;
                charSelect[targetIndex] = targetIndex;
                inputTimerType[targetIndex] = "";
                Destroy(player);
                return;
            }
            Debug.Log($"Returned selection for player {player.GetComponent<PlayerInput>().playerIndex}");
            charSelectLockedIn[targetIndex] = false;
            return;
        }
    }

    //New input system lock in 
    public void LockIn(GameObject player)
    {
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2)
        {
            int targetIndex = player.GetComponent<PlayerInput>().playerIndex;
            bool canLockIn = true;

            for (int i = 0; i < 4; i++)
            {
                if (charSelect[targetIndex] == charSelect[i])
                {
                    if (charSelectLockedIn[i])
                    {
                        canLockIn = false;
                    }
                }
            }
            if (canLockIn)
            {
                //Separate timers for diferent inputs
                if (player.GetComponent<PlayerInput>().currentControlScheme != "Gamepads" && inputTimerKeyboard[targetIndex])
                {
                    if (!charSelectLockedIn[targetIndex])
                    {
                        Debug.Log($"Locked player {player.GetComponent<PlayerInput>().playerIndex} into slot {charSelect[targetIndex]}");
                        charSelectLockedIn[targetIndex] = true;
                        soundmanager.playCharSelectSound(charSelect[targetIndex]);
                        inputTimerKeyboard[targetIndex] = false;
                        InvokeInputTimerKeyboard(targetIndex);
                        return;
                    }
                }
                else if (player.GetComponent<PlayerInput>().currentControlScheme == "Gamepads" && inputTimerController[targetIndex])
                {
                    if (!charSelectLockedIn[targetIndex])
                    {
                        Debug.Log($"Locked player {player.GetComponent<PlayerInput>().playerIndex} into slot {charSelect[targetIndex]}");
                        charSelectLockedIn[targetIndex] = true;
                        soundmanager.playCharSelectSound(charSelect[targetIndex]);
                        inputTimerController[targetIndex] = false;
                        InvokeInputTimerController(targetIndex);

                        //Controller Rumble Test
                        Gamepad gamepad = (Gamepad)player.GetComponent<PlayerInput>().devices[0];
                        gamepad.SetMotorSpeeds(0.5f, 0.5f);
                        StartCoroutine(StopVibration(gamepad, 0.3f));
                        return;
                    }
                }
            }
        }
    }

    //New input system continue
    public void Continue(GameObject player)
    {
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2)
        {
            int targetIndex = player.GetComponent<PlayerInput>().playerIndex;
            if (playersLockedIn >= 2)
            {
                if ( player.GetComponent<PlayerInput>().currentControlScheme != "Gamepads" ||
                     (player.GetComponent<PlayerInput>().currentControlScheme == "Gamepads" && inputTimerController[targetIndex]) )
                {
                    if (!ableToContinue)
                    {
                        Debug.Log("No enough players, bad player amount or joined players not locked in");
                    }
                    else
                    {
                        // set important variables in playerAmount object, so they can be carried to the level
                        playerAmountObj.GetComponent<PlayerAmount>().playerAmount = playersIn;
                        charSelect.CopyTo(playerAmountObj.GetComponent<PlayerAmount>().charSelect, 0);
                        playerIn.CopyTo(playerAmountObj.GetComponent<PlayerAmount>().playerIn, 0);
                        inputTimerType.CopyTo(playerAmountObj.GetComponent<PlayerAmount>().inputTimerType, 0);
                        gameObject.GetComponent<OpeningMenu>().menuState = 3;
                        levelSelectObj.GetComponent<LevelSelect>().InvokeTimer();
                    }
                }
            }
        }
    }
    
    //New input system left/right selection
    public void updateDir(GameObject player, float horizontal)
    {
        if (gameObject.GetComponent<OpeningMenu>().menuState == 2)
        {
            int targetIndex = player.GetComponent<PlayerInput>().playerIndex;
            //Left
            if (horizontal <= -0.9f)
            {
                leftInput[targetIndex] = true;
                return;
            }
            //Right
            else if (horizontal >= 0.9f)
            {
                rightInput[targetIndex] = true;
                return;
            }
            else
            {
                leftInput[targetIndex] = false;
                rightInput[targetIndex] = false;
                return;
            }
        }
    }
}