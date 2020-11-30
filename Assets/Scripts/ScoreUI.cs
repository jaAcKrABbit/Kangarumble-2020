using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    private GameObject mainCam;
    private GameObject sceneCam;
    private GameObject playerVarsObj;
    private GameObject gameEndObj;
    private GameObject timerBarObj;
    private GameObject timerUIObj;

    public GameObject[] playerObj = new GameObject[4];
    public GameObject[] scoreUIParentObj = new GameObject[4];
    public GameObject[] scoreUITextScoreObj = new GameObject[4];
    public GameObject[] scoreUITextScoreLimitObj = new GameObject[4];
    public GameObject[] scoreUITextNameObj = new GameObject[4];
    public GameObject[] scoreUICloudObj = new GameObject[4];
    public GameObject[] scoreUICloudBackObj = new GameObject[4];
    public GameObject[] scoreUIAvatarObj = new GameObject[4];
    public GameObject[] scoreUIBackObj = new GameObject[4];
    public GameObject[] turnIndicatorObj = new GameObject[4];

    public Sprite[] avatarSprite = new Sprite[4];

    public float scoreXOffset = -10;
    public float[] scoreXPos = new float[4];
    public float scoreYPos = 300;
    public float scoreZPos = 0;
    public float scoreTextScoreXOffsetRatio = 5.05f;
    public float scoreTextScoreXOffset = 100;
    public float scoreTextScoreYOffset = 11;
    public float scoreTextNameXOffsetRatio = 5.05f;
    public float scoreTextNameYOffsetRatio = 5.05f;
    public float scoreTextNameXOffset = 100;
    public float scoreTextNameYOffset = 11;
    public float scoreBackXOffsetRatio = 5.05f;
    public float scoreBackXOffset = 100;
    public float scoreBackYOffset = 11;
    public float[] scoreUIShake = new float[4];
    public float[] scoreUIAlertScoreShake = new float[4];
    private float[] alertScoreShakeAmount = new float[3];
    public Color[] alertScoreColor = new Color[3];
    public float turnIndicatorScale = 0.9f;
    private float[] scoreUICloudFillAmount = new float[4];

    public Color textColorMyTurn;
    public Color textColorNotMyTurn;

    public float cloudScaleBig = 1.5f;
    public float cloudScaleSmall = 1f;
    public float avatarScaleRatio = 0.9f;
    public float scorelimitXRatio = 0.03f;
    public float scorelimitYRatio = 0.03f;

    private int playerAmount = 0;
    private float[] scoreUICloudScale = new float[4];
    private float[] scoreUICloudScaleDest = new float[4];

    public Color[] scoreUICloudColor = new Color[4];

    //public int[] playerOrder;
    public bool[] playerIn = new bool[4];
    public int[] charSelect;

    private GameObject countdownObj;
    public float countdownPlusY = -200;

    private GameObject menuControllerObj;
    private int scoreLimit;

    private float currentTime;

    private bool getPlayerIn = false;

    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        sceneCam = GameObject.Find("SceneCamera");
        playerVarsObj = GameObject.Find("PlayerVars");
        countdownObj = GameObject.Find("321Rumble");
        menuControllerObj = GameObject.Find("MenuController");
        gameEndObj = GameObject.Find("GameEnd");
        timerBarObj = GameObject.Find("TimerBar");
        timerUIObj = GameObject.Find("TimerUI");

        // set colors for score UI clouds
        for (int i = 0; i < 4; i++) {
            scoreUICloudObj[i].GetComponent<Image>().color = scoreUICloudColor[i];
            scoreUICloudBackObj[i].GetComponent<Image>().color = BlendColors(scoreUICloudColor[i], Color.black, Color.black);
            turnIndicatorObj[i].SetActive(false);
        }

        // get player order array from PlayerVars object
        //playerOrder = playerVarsObj.GetComponent<PlayerVars>().playerOrder;
        charSelect = playerVarsObj.GetComponent<PlayerVars>().charSelect;
        //playerVarsObj.GetComponent<PlayerVars>().playerIn.CopyTo(playerIn, 0);

        //DeactivateUnusedPlayerUI();

        Invoke("SetScoreLimitText", 0.25f);
        InvokeRepeating("AlertShakePulse1", 2.0f, 2.0f);
        InvokeRepeating("AlertShakePulse2", 1.0f, 1.0f);
        InvokeRepeating("AlertShakePulse3", 0.5f, 0.5f);

        alertScoreShakeAmount[0] = 4;
        alertScoreShakeAmount[1] = 4;
        alertScoreShakeAmount[2] = 4;

        currentTime = 0;
    }

    void Update()
    {
        if (!getPlayerIn)
        {
            playerVarsObj.GetComponent<PlayerVars>().playerIn.CopyTo(playerIn, 0);
            DeactivateUnusedPlayerUI();
            getPlayerIn = true;
        }

        if (countdownObj == null) {
            countdownPlusY = Damp(countdownPlusY, 0, 12f, Time.deltaTime);
        }
        else {
            if (!countdownObj.active) {
                if (countdownPlusY != 0) {
                    countdownPlusY = Damp(countdownPlusY, 0, 12f, Time.deltaTime);
                }
            }
        }
        if (Mathf.Abs(countdownPlusY) < 0.001) {
            countdownPlusY = 0;
        }

        // set ratios for X positions of score UI elements
        scoreTextScoreXOffset = Screen.width / scoreTextScoreXOffsetRatio;
        scoreTextNameXOffset = Screen.width / scoreTextNameXOffsetRatio;
        scoreTextNameYOffset = Screen.height / scoreTextNameYOffsetRatio;
        scoreBackXOffset = Screen.width / scoreBackXOffsetRatio;


        bool isPaused = menuControllerObj.GetComponent<Menus>().isPaused;

        scoreYPos = Screen.height * 0.1f;
        float scoreXSpacing = Screen.width / (playerAmount + 1);
        int iActive = -1;
        for (int i = 0; i < 4; i++) {

            if (!playerIn[i]) {
                continue;
            }
            iActive++;

            GameObject currentPlayerObj = playerObj[i];
            GameObject currentScoreTextScoreObj = scoreUITextScoreObj[i];
            GameObject currentScoreTextScoreLimitObj = scoreUITextScoreLimitObj[i];
            GameObject currentScoreTextNameObj = scoreUITextNameObj[i];
            GameObject currentScoreCloudObj = scoreUICloudObj[i];
            GameObject currentScoreCloudBackObj = scoreUICloudBackObj[i];
            GameObject currentAvatarObj = scoreUIAvatarObj[i];
            GameObject currentScoreBackObj = scoreUIBackObj[i];
            GameObject currentTurnIndicatorObj = turnIndicatorObj[i];

            float currentScoreUIShakeX = Random.Range(-scoreUIShake[i], scoreUIShake[i]);
            float currentScoreUIShakeY = Random.Range(-scoreUIShake[i], scoreUIShake[i]);
            float currentScoreAlertShakeX = Random.Range(-scoreUIAlertScoreShake[i], scoreUIAlertScoreShake[i]);
            float currentScoreAlertShakeY = Random.Range(-scoreUIAlertScoreShake[i], scoreUIAlertScoreShake[i]);

            if (isPaused) {
                currentScoreUIShakeX = 0;
                currentScoreUIShakeY = 0;
            }

            // set text for score UI text
            int currentScore = playerVarsObj.GetComponent<PlayerVars>().playerScores[i];
            string currentName = (playerVarsObj.GetComponent<PlayerVars>().playerNames[charSelect[i]]).ToString();
            currentScoreTextScoreObj.GetComponent<Text>().text = currentScore.ToString();
            currentScoreTextNameObj.GetComponent<Text>().text = currentName;

            // set positions of score UI text, avatar, and cloud
            scoreXPos[i] = scoreXSpacing * (iActive + 1) + scoreXOffset;
            currentScoreCloudObj.transform.position = new Vector3(scoreXPos[i] + currentScoreUIShakeX, scoreYPos + currentScoreUIShakeY + countdownPlusY, scoreZPos);
            currentScoreCloudBackObj.transform.position = currentScoreCloudObj.transform.position;
            currentAvatarObj.transform.position = currentScoreCloudObj.transform.position;
            currentScoreTextScoreObj.transform.position = new Vector3(scoreXPos[i] + scoreTextScoreXOffset + currentScoreUIShakeX + currentScoreAlertShakeX, scoreYPos + scoreTextScoreYOffset + currentScoreUIShakeY + currentScoreAlertShakeY + countdownPlusY, scoreZPos);
            currentScoreTextScoreLimitObj.transform.position = new Vector3(currentScoreTextScoreObj.transform.position.x - currentScoreAlertShakeX + (Screen.width * scorelimitXRatio), currentScoreTextScoreObj.transform.position.y - currentScoreAlertShakeY + (Screen.height * scorelimitYRatio), currentScoreTextScoreObj.transform.position.z);
            currentScoreTextNameObj.transform.position = new Vector3(scoreXPos[i] + scoreTextNameXOffset + currentScoreUIShakeX, scoreYPos + scoreTextNameYOffset + currentScoreUIShakeY + countdownPlusY, scoreZPos);
            currentScoreBackObj.transform.position = new Vector3(scoreXPos[i] + scoreBackXOffset + currentScoreUIShakeX, scoreYPos + scoreBackYOffset + currentScoreUIShakeY + countdownPlusY, scoreZPos);

            // set avatar sprite
            currentAvatarObj.GetComponent<Image>().sprite = avatarSprite[charSelect[i]];

            bool currentMyTurn = false;

            // set target size for cloud UI
            if (currentPlayerObj == null) {
                // if we cannot find the player in the scene, set size of cloud to small and color of UI to default
                scoreUICloudScaleDest[i] = cloudScaleSmall;
                currentScoreTextScoreObj.GetComponent<Text>().color = textColorNotMyTurn;
                currentScoreTextNameObj.GetComponent<Text>().color = textColorNotMyTurn;
            }
            else {
                // if we can find the player in the scene, set size & color of score UI depending on whether it is their turn
                currentMyTurn = currentPlayerObj.GetComponent<Player>().myTurn;
                scoreUICloudScaleDest[i] = (currentMyTurn) ? cloudScaleBig : cloudScaleSmall;
                currentScoreTextScoreObj.GetComponent<Text>().color = (currentMyTurn) ? textColorMyTurn : textColorNotMyTurn;
                currentScoreTextNameObj.GetComponent<Text>().color = (currentMyTurn) ? textColorMyTurn : textColorNotMyTurn;

                // make the cloud back obj have clock timer effect
                if (currentMyTurn) {
                    float timerCurrentFillAmount = timerUIObj.GetComponent<TimerUI>().currentFillAmount;
                    float timerBarScale = timerUIObj.GetComponent<TimerUI>().barScale;
                    if (timerCurrentFillAmount > timerBarScale) {
                        float currentTimerBarFill = timerBarObj.GetComponent<Image>().fillAmount;
                        currentScoreCloudObj.GetComponent<Image>().fillAmount = currentTimerBarFill;
                    }
                }
                else {
                    float currentFillAmount = currentScoreCloudObj.GetComponent<Image>().fillAmount;
                    currentFillAmount = Damp(currentFillAmount, 1, 10f, Time.deltaTime);
                    currentScoreCloudObj.GetComponent<Image>().fillAmount = currentFillAmount;
                }

                // if this player's score is close to the score limit, we will color the score text differently
                if (currentScore > 0) {
                    if (scoreLimit - currentScore == 3) {
                        currentScoreTextScoreObj.GetComponent<Text>().color = alertScoreColor[0];
                    }
                    else if (scoreLimit - currentScore == 2) {
                        currentScoreTextScoreObj.GetComponent<Text>().color = alertScoreColor[1];
                    }
                    else if (scoreLimit - currentScore <= 1) {
                        currentScoreTextScoreObj.GetComponent<Text>().color = alertScoreColor[2];
                    }
                }


                // turn indicator
                turnIndicatorObj[i].SetActive(false);
                if (currentMyTurn) {
                    turnIndicatorObj[i].transform.position = currentScoreCloudObj.transform.position;
                    turnIndicatorObj[i].transform.Rotate(0, 0, 50 * Time.deltaTime);
                    turnIndicatorObj[i].transform.localScale = new Vector3(turnIndicatorScale, turnIndicatorScale, 1);
                }
            }
            scoreUICloudScale[i] = Damp(scoreUICloudScale[i], scoreUICloudScaleDest[i], 14f, Time.deltaTime);
            if (currentMyTurn) {
                scoreUICloudScale[i] = (Mathf.Sin(currentTime * 10) / 16) + 0.75f;
            }
            currentScoreCloudObj.transform.localScale = new Vector3(scoreUICloudScale[i], scoreUICloudScale[i], 1);
            currentScoreCloudBackObj.transform.localScale = currentScoreCloudObj.transform.localScale;

            // scale avatar
            currentAvatarObj.transform.localScale = (currentScoreCloudObj.transform.localScale * avatarScaleRatio);

            // decrease shake effect
            scoreUIShake[i] = Mathf.Max(scoreUIShake[i] - (20f * Time.deltaTime), 0);
            scoreUIAlertScoreShake[i] = Mathf.Max(scoreUIAlertScoreShake[i] - (20f * Time.deltaTime), 0);
        }
    }

    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
    }

    
    void DeactivateUnusedPlayerUI()
    {
        // make sure we are not showing score UI for deactivated players
        playerAmount = playerVarsObj.GetComponent<PlayerVars>().playerAmount;

        for (var i = 0; i < 4; i++) {
            if (!playerIn[i]) {
                scoreUITextScoreObj[i].SetActive(false);
                scoreUITextNameObj[i].SetActive(false);
                scoreUICloudObj[i].SetActive(false);
                scoreUICloudBackObj[i].SetActive(false);
                scoreUIBackObj[i].SetActive(false);
                scoreUIAvatarObj[i].SetActive(false);
                scoreUITextScoreLimitObj[i].SetActive(false);
            }
        }
    }

    void SetScoreLimitText()
    {
        scoreLimit = mainCam.GetComponent<CheckForWinner>().scoreLimit;
        for (int i = 0; i < 4; i++) {
            GameObject currentScoreTextScoreLimitObj = scoreUITextScoreLimitObj[i];
            currentScoreTextScoreLimitObj.GetComponent<Text>().text = "/ " + scoreLimit;
        }
    }

    void AlertShakePulse1()
    {
        if (!gameEndObj.GetComponent<GameEnd>().gameEnd) {
            for (int i = 0; i < 4; i++) {
                int currentScore = playerVarsObj.GetComponent<PlayerVars>().playerScores[i];
                if (scoreLimit - currentScore == 3 && currentScore > 0) {
                    scoreUIAlertScoreShake[i] = alertScoreShakeAmount[0];
                }
            }
        }
    }
    void AlertShakePulse2()
    {
        if (!gameEndObj.GetComponent<GameEnd>().gameEnd) {
            for (int i = 0; i < 4; i++) {
                int currentScore = playerVarsObj.GetComponent<PlayerVars>().playerScores[i];
                if (scoreLimit - currentScore == 2 && currentScore > 0) {
                    scoreUIAlertScoreShake[i] = alertScoreShakeAmount[1];
                }
            }
        }
    }
    void AlertShakePulse3()
    {
        if (!gameEndObj.GetComponent<GameEnd>().gameEnd) {
            for (int i = 0; i < 4; i++) {
                int currentScore = playerVarsObj.GetComponent<PlayerVars>().playerScores[i];
                if (scoreLimit - currentScore == 1 && currentScore > 0) {
                    scoreUIAlertScoreShake[i] = alertScoreShakeAmount[2];
                }
            }
        }
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    public static Color BlendColors(params Color[] colors)
    {
        Color blend = new Color(0,0,0,0);
        foreach(Color c in colors) {
            blend += c;
        }
        blend /= colors.Length;
        return blend;
    }
}
