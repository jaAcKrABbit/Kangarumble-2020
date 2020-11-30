using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LevelSettings : MonoBehaviour
{
    public int levelCurrent = 0;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode upKey;
    public KeyCode downKey;

    [SerializeField] private bool inputLeft = false;
    [SerializeField] private bool inputRight = false;
    [SerializeField] private bool inputUp = false;
    [SerializeField] private bool inputDown = false;
    //private bool inputEnter = false;
    //[SerializeField] private bool inputLeftController = false;
    //[SerializeField] private bool inputRightController = false;
    //[SerializeField] private bool inputUpController = false;
    //[SerializeField] private bool inputDownController = false;
    //private bool inputButtonA = false;
    private bool hideTexts = true;
    private bool lerpA = false;
    private bool lerpB = false;
    private bool lerpC = false;

    //public KeyCode advanceKeyCode;
    //public KeyCode advanceKeyCodeController;
    //private bool inputAdvance;

    private GameObject playerAmountObj;
    private GameObject openingMenuControlObj;
    private GameObject GameModetxt;
    private GameObject Modetxt;
    private GameObject UI;
    private GameObject arrowA;
    private GameObject arrowB;
    private GameObject arrowC;
    private GameObject arrowD;
    private GameObject Rumble;
    private GameObject Goal;
    private GameObject Score;
    private GameObject transitionManager;


    //public bool inputTimerKeyboardLR = true;
    //public bool inputTimerControllerLR = true;
    //public bool inputTimerKeyboardUD = true;
    //public bool inputTimerControllerUD = true;



    //public float inputDelayTimeKeyboardLR;
    //public float inputDelayTimeControllerLR;
    //public float inputDelayTimeKeyboardUD;
    //public float inputDelayTimeControllerUD;


    //LEVEL DATA
    public int scoreLimit = 10;
    private int selector = 0;
    private int modeSelector = 0;
    private int last = -1;
    public float timeLimitMin = 1f;
    public float timeLimitMax = 5f;
    public float timeLimitDec = 0.3f;
    private float scaleOriginal = 1;
    private float scaleDest = 1.1f;
    public float smoothRate = 20f;
    public float scaleOrigin = 1;
    public float scaleDes = 1.1f;

    [SerializeField]
    private bool TestCheckBox;

    public bool inputTimer;
    public float inputDelayTime;

    void Start()
    {
        //TODO: Set up LevelSettings UI Elements and make them invisible
        /*

        */
        playerAmountObj = GameObject.Find("PlayerAmountObj");
        openingMenuControlObj = GameObject.Find("GameTitle");
        UI = GameObject.Find("CustomSettings");
        GameModetxt = GameObject.Find("GameMode");
        Modetxt = GameObject.Find("Mode");
        arrowA = GameObject.Find("AR");
        arrowB = GameObject.Find("AL");
        arrowC = GameObject.Find("aR");
        arrowD = GameObject.Find("aL");
        Rumble = GameObject.Find("Rumble");
        Goal = GameObject.Find("ScoreLimit");
        Score = GameObject.Find("Score");
        transitionManager = GameObject.Find("SceneTransition");
        GameModetxt.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        // Remove TestCheckBox after implemented
        //   TestCheckBox = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (openingMenuControlObj.GetComponent<OpeningMenu>().menuState != 4)
        {
            inputTimer = false;
        }

        if (hideTexts) {
            UI.SetActive(false);
        } else {
            UI.SetActive(true);
        }
        if (openingMenuControlObj.GetComponent<OpeningMenu>().menuState != 4){
            // RESET VARIABLES ONCE LEAVING STATE 4 TO DEFAULT
            scoreLimit = 10; // Example
            timeLimitMin = 1f;
            timeLimitMax = 5f;
            timeLimitDec = 0.3f;
            hideTexts = true;
            // Remove TestCheckBox after implemented
            //TestCheckBox = false;
        }
        if (openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 4){
            //inputLeftController = false;
            //inputRightController = false;
            //inputDownController = false;
            //inputUpController = false;
            hideTexts = false;
            //CAUTION: all idle controllers could control level left/right
            //if (Input.GetAxisRaw("HorizontalC") <= -0.75f || Input.GetAxis("XboxDpadHorizontal") < -0.5f)
            //{
            //    inputLeftController = true;
            //}
            //else if (Input.GetAxisRaw("HorizontalC") >= 0.75f || Input.GetAxis("XboxDpadHorizontal") > 0.5f)
            //{
            //    inputRightController = true;
            //}
            //if (Input.GetAxisRaw("VerticalC") >= 0.75f  || Input.GetAxis("XboxDpadVertical") < -0.5f)
            //{
            //    inputDownController = true;
            //}
            //else if (Input.GetAxisRaw("VerticalC") <= -0.75f || Input.GetAxis("XboxDpadVertical") > 0.5f)
            //{
            //    inputUpController = true;
            //}
            //inputLeft = Input.GetKey(leftKey);
            //inputRight = Input.GetKey(rightKey);
            //inputDown = Input.GetKey(downKey);
            //inputUp = Input.GetKey(upKey);
            //inputEnter = Input.GetKey(KeyCode.Return);
            //inputButtonA = Input.GetKey(KeyCode.JoystickButton0); //A 1 is B

            //New Input System Right
            if (inputTimer && inputRight)
            {
                inputTimer = false;
                switch (selector)
                {
                    //Mode switch
                    case 0:
                        if (modeSelector < 2) modeSelector++;
                        else modeSelector = 0;
                        SwitchMode();
                        break;
                    //Score Limit
                    case 1:
                        scoreLimit++;
                        if (scoreLimit > 30) scoreLimit = 1;
                        Score.GetComponent<Text>().text = scoreLimit.ToString();
                        break;
                }
                Invoke("ResetInputTimer", inputDelayTime);
            }

            //New Input System Left
            if (inputTimer && inputLeft)
            {
                inputTimer = false;
                switch (selector)
                {
                    case 0:
                        if (modeSelector > 0) modeSelector--;
                        else modeSelector = 2;
                        SwitchMode();
                        break;
                    case 1:
                        scoreLimit--;
                        if (scoreLimit < 1) scoreLimit = 30;
                        Score.GetComponent<Text>().text = scoreLimit.ToString();
                        break;
                }
                Invoke("ResetInputTimer", inputDelayTime);
            }
            //New Input System Up
            if (inputTimer && inputUp)
            {
                inputTimer = false;
                // selector logic
                if (selector > 0) selector--;
                else selector = 2;
                Invoke("ResetInputTimer", inputDelayTime);
            }
            //New Input System Down
            if (inputTimer && inputDown)
            {
                inputTimer = false;
                // selector logic
                if (selector < 2) selector++;
                else selector = 0;

                Invoke("ResetInputTimer", inputDelayTime);
            }


            ////Keyboard right
            //if (inputTimerKeyboardLR && inputRight){
            //    inputTimerKeyboardLR = false;
            //    switch (selector) {
            //        //Mode switch
            //        case 0:
            //            if (modeSelector < 2) modeSelector++;
            //            else modeSelector = 0;
            //            SwitchMode();
            //            break;
            //        //Score Limit
            //        case 1:
            //            scoreLimit++;
            //            if (scoreLimit > 30) scoreLimit = 1;
            //            Score.GetComponent<Text>().text = scoreLimit.ToString();
            //            break;
            //    }
            //    Invoke("ResetInputTimerKeyboardLR", inputDelayTimeKeyboardLR);
            //}
            ////Keyboard left
            //if (inputTimerKeyboardLR && inputLeft){
            //    inputTimerKeyboardLR = false;
            //    switch (selector) {
            //        case 0:
            //            if (modeSelector > 0) modeSelector--;
            //            else modeSelector = 2;
            //            SwitchMode();
            //            break;
            //        case 1:
            //            scoreLimit--;
            //            if (scoreLimit <1) scoreLimit = 30;  
            //            Score.GetComponent<Text>().text = scoreLimit.ToString();
            //            break;
            //    }
            //    Invoke("ResetInputTimerKeyboardLR", inputDelayTimeKeyboardLR);
            //}
            ////Controller right
            //if (inputTimerControllerLR && inputRightController){
            //    inputTimerControllerLR = false;
            //    switch (selector) {
            //        //Mode switch
            //        case 0:
            //            if (modeSelector < 2) modeSelector++;
            //            else modeSelector = 0;
            //            SwitchMode();
            //            break;
            //        //Score Limit
            //        case 1:
            //            scoreLimit++;
            //            if (scoreLimit > 30) scoreLimit = 1;
            //            Score.GetComponent<Text>().text = scoreLimit.ToString();
            //            break;
            //    }
            //    Invoke("ResetInputTimerControllerLR", inputDelayTimeControllerLR);
            //}
            ////Controller left
            //if (inputTimerControllerLR && inputLeftController){

            //    inputTimerControllerLR = false;
            //    switch (selector) {
            //        case 0:
            //            if (modeSelector > 0) modeSelector--;
            //            else modeSelector = 2;
            //            SwitchMode();
            //            break;
            //        case 1:
            //            scoreLimit--;
            //            if (scoreLimit < 1) scoreLimit = 30;
            //            Score.GetComponent<Text>().text = scoreLimit.ToString();
            //            break;
            //    }
            //    Invoke("ResetInputTimerControllerLR", inputDelayTimeControllerLR);
            //}
            ////Keyboard down
            //if (inputTimerKeyboardUD && inputDown){
            //    inputTimerKeyboardUD = false;
            //    // selector logic
            //    if (selector < 2) selector++;
            //    else selector = 0;
            //    Invoke("ResetInputTimerKeyboardUD", inputDelayTimeKeyboardUD);
            //}
            ////Keyboard up
            //if (inputTimerKeyboardUD && inputUp){
            //    inputTimerKeyboardUD = false;
            //    // selector logic
            //    if (selector > 0) selector--;
            //    else selector = 2;
            //    Invoke("ResetInputTimerKeyboardUD", inputDelayTimeKeyboardUD);
            //}
            ////Controller down
            //if (inputTimerControllerUD && inputDownController){
            //    inputTimerControllerUD = false;
            //    // selector logic
            //    if (selector < 2) selector++;
            //    else selector = 0;
            //    Invoke("ResetInputTimerControllerUD", inputDelayTimeControllerUD);
            //}
            ////Controller up
            //if (inputTimerControllerUD && inputUpController){

            //    inputTimerControllerUD = false;
            //    // selector logic
            //    if (selector > 0) selector--;
            //    else selector = 2;
            //    Invoke("ResetInputTimerControllerUD", inputDelayTimeControllerUD);
            //}

            // change scale of turn text
            scaleOriginal = LevelSelect.Damp(scaleOriginal,scaleDes, smoothRate, Time.deltaTime);
            scaleDest = LevelSelect.Damp(scaleDest, scaleOrigin, smoothRate, Time.deltaTime);
            //Simple UI effects here
            switch (selector) {
                case 0:
                    if(last == 2) {
                        Rumble.GetComponent<RectTransform>().localScale = new Vector3(scaleDest, scaleDest, scaleDest);
                    }else if(last == 1) {
                        Goal.GetComponent<RectTransform>().localScale = new Vector3(scaleDest, scaleDest, scaleDest);
                    }
                    GameModetxt.GetComponent<RectTransform>().localScale = new Vector3(scaleOriginal, scaleOriginal, scaleOriginal);
                    //color outline
                    GameModetxt.GetComponent<Outline>().effectColor = new Color(1, 0.92f, 0.016f, 1);
                    Goal.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1);
                    Rumble.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1);
                    last = 0;
                    break;
                case 1:
                    if (last == 2) {
                        Rumble.GetComponent<RectTransform>().localScale = new Vector3(scaleDest, scaleDest, scaleDest);
                    } else if (last == 0) {
                        GameModetxt.GetComponent<RectTransform>().localScale = new Vector3(scaleDest, scaleDest, scaleDest);
                    }
                    Goal.GetComponent<RectTransform>().localScale = new Vector3(scaleOriginal, scaleOriginal, scaleOriginal);  
                    //color outline
                    GameModetxt.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1);
                    Goal.GetComponent<Outline>().effectColor = new Color(1, 0.92f, 0.016f, 1);
                    Rumble.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1);
                    last = 1;
                    break;
                case 2:
                    if (last == 1) {
                        Goal.GetComponent<RectTransform>().localScale = new Vector3(scaleDest, scaleDest, scaleDest);
                    } else if (last == 0) {
                        GameModetxt.GetComponent<RectTransform>().localScale = new Vector3(scaleDest, scaleDest, scaleDest);
                    }
                    Rumble.GetComponent<RectTransform>().localScale = new Vector3(scaleOriginal, scaleOriginal, scaleOriginal);
                    //color outline
                    GameModetxt.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1);
                    Goal.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1);
                    Rumble.GetComponent<Outline>().effectColor = new Color(1, 0.92f, 0.016f, 1);
                    last = 2;
                    break;
            }
            ////go to next scene
            //if (selector == 2 && (inputEnter || inputButtonA)) {
            //    //SceneManager.LoadScene("Loading");
            //    transitionManager.GetComponent<TransitionManagerMenu>().MenuEndClose();
            //}

        }
    }
    //public void ResetInputTimerKeyboardLR(){
    //    inputTimerKeyboardLR = true;
    //}
    //public void ResetInputTimerControllerLR(){
    //    inputTimerControllerLR = true;
    //}
    //public void ResetInputTimerKeyboardUD() {
    //    inputTimerKeyboardUD = true;
    //}
    //public void ResetInputTimerControllerUD() {
    //    inputTimerControllerUD = true;
    //}

    void SwitchMode() {
        switch (modeSelector) {
            case 0:
                Modetxt.GetComponent<Text>().text = "NORMAL";//temp name
                timeLimitMin = 1;
                timeLimitMax = 5;
                timeLimitDec = 0.3f;
                break;
            case 1:
                Modetxt.GetComponent<Text>().text = "MILD";
                timeLimitMin = 1;
                timeLimitMax = 10;
                timeLimitDec = 0.2f;
                break;
            case 2:
                Modetxt.GetComponent<Text>().text = "CRAZY";
                timeLimitMin = 0.5f;
                timeLimitMax = 3f;
                timeLimitDec = 0.2f;
                break;
        }
    }

    public void InvokeTimer()
    {
        Invoke("ResetInputTimer", inputDelayTime);
    }

    public void ResetInputTimer()
    {
        inputTimer = true;
    }

    
    //New input system 2-Axis selection
    public void updateDir(Vector2 data)
    {
        if (openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 4)
        {
            //Left
            if (data.x <= -0.9f)
            {
                inputLeft = true;
                return;
            }
            //Right
            else if (data.x >= 0.9f)
            {
                inputRight = true;
                return;
            }
            //Up
            else if (data.y >= 0.9f)
            {
                inputUp = true;
                return;
            }
            //Down
            else if (data.y <= -0.9f)
            {
                inputDown = true;
                return;
            }
            else
            {
                inputLeft = false;
                inputRight = false;
                inputUp = false;
                inputDown = false;
                return;
            }
        }
    }
    

    //New input system continue to Loading
    public void Continue()
    {
        //take you to level settings
        if (inputTimer && openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 4)
        {
            Debug.Log("Loading level " + levelCurrent + "...");
            string currentLevelName = "";
            if (levelCurrent == 0)
            {
                currentLevelName = "TestLevel";
            }
            else if (levelCurrent == 1)
            {
                currentLevelName = "Industrial";
            }
            else if (levelCurrent == 2)
            {
                currentLevelName = "Carnival";
            }

            //pass level settings data to playerAmountObj.GetComponent<PlayerAmount>()
            playerAmountObj.GetComponent<PlayerAmount>().scoreLimit = scoreLimit;
            playerAmountObj.GetComponent<PlayerAmount>().timeLimitMin = timeLimitMin;
            playerAmountObj.GetComponent<PlayerAmount>().timeLimitMax = timeLimitMax;
            playerAmountObj.GetComponent<PlayerAmount>().timeLimitDec = timeLimitDec;
            playerAmountObj.GetComponent<PlayerAmount>().currentLevelName = currentLevelName;

            if (selector == 2)
            {
                transitionManager.GetComponent<TransitionManagerMenu>().MenuEndClose();
            }
        }
    }

    //New input system gamepad Back 
    public void Back()
    {
        if (inputTimer && openingMenuControlObj.GetComponent<OpeningMenu>().menuState == 4)
        {
            //take you to level settings
            openingMenuControlObj.GetComponent<OpeningMenu>().menuState = 3;
            gameObject.GetComponent<LevelSelect>().InvokeTimer();
        }
    }
}