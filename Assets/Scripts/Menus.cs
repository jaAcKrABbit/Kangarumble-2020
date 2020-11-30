using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menus : MonoBehaviour{
    [SerializeField] 
    private GameObject pauseMenuUI;
    [SerializeField]
    private GameObject settingsUI;
    [SerializeField]
    public bool isPaused;
    [SerializeField]
    public bool controllerInput;
    private GameObject countdown;
    private GameObject gameEndObj;
    private bool readyToPause;
    private bool settingsActive = false;
    private int selector = 0;
    private int audioSelector = 0;
    private GameObject playerVarsObj;
    //private float inputTimer = 0;
    private bool controlGet = false;

    public bool inputTimer;
    public float inputDelayTime;

    private bool inputLeft = false;
    private bool inputRight = false;
    private bool inputUp = false;
    private bool inputDown = false;

    void Start() {
        countdown = GameObject.Find("321Rumble");
        playerVarsObj = GameObject.Find("PlayerVars");
        gameEndObj = GameObject.Find("GameEnd");
        inputTimer = true;
    }

    void Update() {

        if (!controlGet && playerVarsObj.GetComponent<PlayerVars>().loadDone)
        {
            //controllerInput = playerVarsObj.GetComponent<PlayerVars>().controllerInput;
        }

        //if (!isPaused)
        //{
        //    inputTimer = false;
        //}

        //Moved to pause()
        //if (inputTimer <1 && ( (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) && !settingsActive && !countdown.activeInHierarchy) ){
        //    isPaused = !isPaused;
        //    //inputTimer = 20;
        //    inputTimer = false;
        //    Invoke("ResetInputTimer", inputDelayTime);
        //}

        if (isPaused && !settingsActive) {
            OnPause();
            Selection();
        } else if (!isPaused) {
            OnResume();
        }
        if (settingsActive) {
            AudioControl();
            //if (inputTimer < 1 && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1))) {
            //    goBackToPause();
            //    inputTimer = 20;
            //} 
        }
    }

    //New Timer
    //public void InvokeTimer()
    //{
    //    Invoke("ResetInputTimer", inputDelayTime);
    //}

    //Uses realtime coroutine to avoid scaled time paused in game
    IEnumerator ResetInputTimer(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        inputTimer = true;
    }

    public void OnPause(){
        Time.timeScale = 0;
 //       AudioListener.pause = true;
        pauseMenuUI.SetActive(true);
    }
    public void OnResume() {
        
        Time.timeScale = 1f;
   //     AudioListener.pause = false;
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }
    public void OnRestart() {
        SceneManager.LoadScene("OpeningMenu");
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Quit() {
        Application.Quit();
    }
    
    private void gotoSettings() {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
        settingsActive = true;
    }
    private void goBackToPause() {
        settingsUI.SetActive(false);
        settingsActive = false;
        isPaused = true;
        pauseMenuUI.SetActive(true);
    }

    public void Selection() {

        //Up
        if (inputTimer && inputUp )
        {
            inputTimer = false;
            StartCoroutine(ResetInputTimer(inputDelayTime));
            selector--;
            if (selector < 0) selector = 3;
        }

        //Down
        if (inputTimer && inputDown )
        {
            inputTimer = false;
            StartCoroutine(ResetInputTimer(inputDelayTime));
            selector++;
            if (selector > 3) selector = 0;
        }

        GameObject resumeButton = GameObject.Find("Resume");
        GameObject restartButton = GameObject.Find("Restart");
        GameObject quitButton = GameObject.Find("Quit");
        GameObject settingButton = GameObject.Find("Settings");
        switch (selector) {
            case 0:
                restartButton.GetComponent<Outline>().enabled = false;
                resumeButton.GetComponent<Outline>().enabled = true;
                quitButton.GetComponent<Outline>().enabled = false;
                settingButton.GetComponent<Outline>().enabled = false;
                //if (inputTimer < 1 && ( Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0) ))
                //{
                //    OnResume();
                //}
                break;
            case 1:
                restartButton.GetComponent<Outline>().enabled = false;
                resumeButton.GetComponent<Outline>().enabled = false;
                quitButton.GetComponent<Outline>().enabled = false;
                settingButton.GetComponent<Outline>().enabled = true;
                //if (inputTimer < 1 && ( Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0) ))
                //{
                //    inputTimer = 20;
                //    gotoSettings();
                //}
                break;
            case 2:
                restartButton.GetComponent<Outline>().enabled = true;
                resumeButton.GetComponent<Outline>().enabled = false;
                quitButton.GetComponent<Outline>().enabled = false;
                settingButton.GetComponent<Outline>().enabled = false;
                //if (inputTimer < 1 && ( Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0) ))
                //{
                //    OnRestart();
                //}
                break;
            case 3:
                restartButton.GetComponent<Outline>().enabled = false;
                resumeButton.GetComponent<Outline>().enabled = false;
                quitButton.GetComponent<Outline>().enabled = true;
                settingButton.GetComponent<Outline>().enabled = false;
                //if (inputTimer < 1 && ( Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0) ))
                //{
                //    Quit();
                //}
                break;
        }
    }

    public void AudioControl() {
        if (inputTimer && inputUp)
        {
            inputTimer = false;
            StartCoroutine(ResetInputTimer(inputDelayTime));
            audioSelector--;
            if (audioSelector < 0) audioSelector = 1;
        }
        if (inputTimer && inputDown) {
            inputTimer = false;
            StartCoroutine(ResetInputTimer(inputDelayTime));
            audioSelector++;
            if (audioSelector > 1) audioSelector = 0;
        }
        GameObject bgmSlider = GameObject.Find("BGM");
        GameObject sfxSlider = GameObject.Find("SFX");
        GameObject sfxtxt = GameObject.Find("Sfxtxt");
        GameObject mtxt = GameObject.Find("mtxt");

        switch (audioSelector) {
            case 0:
                mtxt.GetComponent<Outline>().effectColor = new Color(1, 0.92f, 0.016f, 1);
                sfxtxt.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1);
                //Right
                if (inputTimer && inputRight)
                {
                    if(bgmSlider.GetComponent<Slider>().value < 1) {
                        bgmSlider.GetComponent<Slider>().value += 0.01f;
                    }   
                }
                //Left
                if (inputTimer && inputLeft)
                {
                    if (bgmSlider.GetComponent<Slider>().value > 0) {
                        bgmSlider.GetComponent<Slider>().value -= 0.01f;
                    }
                }
                break;
            case 1:
                sfxtxt.GetComponent<Outline>().effectColor = new Color(1, 0.92f, 0.016f, 1);
                mtxt.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 1);
                //Right
                if (inputTimer && inputRight)
                {
                    if (sfxSlider.GetComponent<Slider>().value < 1) {
                        sfxSlider.GetComponent<Slider>().value += 0.01f;
                    }
                }
                //Left
                if (inputTimer && inputLeft)
                {
                    if (sfxSlider.GetComponent<Slider>().value > 0) {
                        sfxSlider.GetComponent<Slider>().value -= 0.01f;
                    }
                }
                break;
        }
    }

    //New input system
    public void Pause()
    {
        bool gameEnd = gameEndObj.GetComponent<GameEnd>().gameEnd;
        if (inputTimer && !settingsActive && !countdown.activeInHierarchy && !gameEnd)
        {
            isPaused = !isPaused;
            inputTimer = false;
            StartCoroutine(ResetInputTimer(inputDelayTime));
        }
    }

    public void Back()
    {
        if (inputTimer && settingsActive)
        {
            goBackToPause();
            inputTimer = false;
            StartCoroutine(ResetInputTimer(inputDelayTime));
        }
    }

    public void updateDir(Vector2 data)
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

    public void Continue()
    {
        //take you to level settings
        if (isPaused && !settingsActive)
        {
            switch (selector)
            {
                case 0:
                    if (inputTimer)
                    {
                        OnResume();
                    }
                    break;
                case 1:
                    if (inputTimer)
                    {
                        inputTimer = false;
                        StartCoroutine(ResetInputTimer(inputDelayTime));
                        gotoSettings();
                    }
                    break;
                case 2:
                    if (inputTimer)
                    {
                        OnRestart();
                    }
                    break;
                case 3:
                    if (inputTimer)
                    {
                        Quit();
                    }
                    break;
            }
        }
    }
}
