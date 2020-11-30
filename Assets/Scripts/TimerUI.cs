using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public float timerYMultiplier = 0.27f;

    public float turnTextScale = 0;
    public float turnTextScaleDest = 1;
    public string playerTurnTextStr = "";
    public Color timerBarColor;

    private GameObject camera;
    private GameObject timerBarObj;
    private GameObject timerBorderObj;
    private GameObject timerBackObj;
    private GameObject timerCloudRightObj;
    private GameObject timerCloudRightShadowObj;
    private GameObject timeRemainingIntObj;
    private GameObject timeRemainingDecObj;
    private GameObject playerTurnTextObj;
    private GameObject playerVarsObj;
    private GameObject uiObj;
    private GameObject countdownObj;
    private GameObject turnIndicatorObj;
    private GameObject menuControllerObj;
    private GameObject gameEndObj;

    public float currentFillAmount;
    public float barScale;
    private float currentTime = 0;
    private float timerBarX = 0;
    private float timerBarY = 0;
    private float timerBarYDest = 0;
    private float timerCloudRightX = 0;
    private float timerCloudRightY = 0;

    public float textShake = 0;

    private bool firstGrow = false;

    private int initUpdateTextCounter = 5;
    private bool initUpdateText = false;

    private float dampSmoothRate = 20f;

    public float timerCloudScale;
    public float timerCloudScaleMin;
    public float timerCloudScaleMax;
    public float timerCloudXRatio;
    
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        timerBarObj = GameObject.Find("TimerBar");
        timerBackObj = GameObject.Find("TimerBarBG");
        timerCloudRightObj = GameObject.Find("TimerCloudRight");
        timerCloudRightShadowObj = GameObject.Find("TimerCloudRightShadow");
        timeRemainingIntObj = GameObject.Find("TimeRemainingTextInteger");
        timeRemainingDecObj = GameObject.Find("TimeRemainingTextDecimal");
        playerTurnTextObj = GameObject.Find("PlayerTurnText");
        playerVarsObj = GameObject.Find("PlayerVars");
        uiObj = GameObject.Find("UI");
        turnIndicatorObj = GameObject.Find("TurnIndicatorSprite");
        menuControllerObj = GameObject.Find("MenuController");
        gameEndObj = GameObject.Find("GameEnd");

        //turnIndicatorObj.SetActive(false);

        // set origin positions
        countdownObj = GameObject.Find("321Rumble");
        timerBarYDest = Screen.height;
        timerBarY = timerBarYDest;
        timerBarObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(timerBarX, timerBarY, 0);
        timerBackObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(timerBarX, timerBarY, 0);
        playerTurnTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(timerBarX, timerBarY, 0);

        UpdateText();

        currentTime = 0;
    }

    
    void Update()
    {
        currentTime += Time.deltaTime;

        if (!countdownObj.active) {
            turnIndicatorObj.SetActive(true);
        }

        // color of bar is the current player color
        int currentPlayerTurn = camera.GetComponent<TurnSwitch>().currentPlayerTurn;
        Color currentPlayerColor = uiObj.GetComponent<ScoreUI>().scoreUICloudColor[currentPlayerTurn];
        Color currentTimerBarColor = timerBarObj.GetComponent<Image>().color;
        timerBarColor = ColorDamp(currentTimerBarColor, currentPlayerColor, 7f, Time.deltaTime);
        timerBarObj.GetComponent<Image>().color = timerBarColor;
        Color timerBackColor = BlendColors(currentTimerBarColor, Color.black, Color.black, Color.black, Color.black, Color.black);
        timerBackObj.GetComponent<Image>().color = timerBackColor;


        // only change text if the scale is tiny and can't be seens
        if (turnTextScale < 0.01f) {
            UpdateText();
            textShake = 0;
        }

        // change x-scale of timer UI bar
        float currentPlayerTimeFull = camera.GetComponent<TurnSwitch>().currentPlayerTimeFull;
        float currentPlayerTimeRemaining = camera.GetComponent<TurnSwitch>().currentPlayerTimeRemaining;
        barScale = currentPlayerTimeRemaining / currentPlayerTimeFull;
        currentFillAmount = timerBarObj.GetComponent<Image>().fillAmount;
        if (currentFillAmount > barScale) {
            timerBarObj.GetComponent<Image>().fillAmount = barScale;
        }
        else if (currentFillAmount < barScale) {
            float newBarScale = Damp(currentFillAmount, barScale, 18f, Time.deltaTime);
            timerBarObj.GetComponent<Image>().fillAmount = newBarScale;
        }

        // change scale of turn text
        turnTextScale = Damp(turnTextScale, turnTextScaleDest, dampSmoothRate, Time.deltaTime);


        // set shake amount
        float textShakeX = Random.Range(-textShake, textShake);
        float textShakeY = Random.Range(-textShake, textShake);
        if (menuControllerObj.GetComponent<Menus>().isPaused) {
            textShakeX = 0;
            textShakeY = 0;
        }

        // set position of timer bar and text
        timerBarX = textShakeX;
        if (countdownObj.active) {
            timerBarYDest = Screen.height;
        }
        else {
            timerBarYDest = (Screen.height * timerYMultiplier) + textShakeY;
        }
        timerBarY = Damp(timerBarY, timerBarYDest, dampSmoothRate, Time.deltaTime);

        timerBarObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(timerBarX, timerBarY, 0);
        timerBackObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(timerBarX, timerBarY, 0);
        playerTurnTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(timerBarX, timerBarY, 0);
        playerTurnTextObj.GetComponent<RectTransform>().localScale = new Vector3(turnTextScale, turnTextScale, 1);

        if (initUpdateTextCounter > 0) {
            initUpdateTextCounter--;
        }
        else if (!initUpdateText) {
            initUpdateText = true;
            Debug.Log("initial UpdateText");
            UpdateText();
        }

        // set position and scale of timer cloud
        timerCloudRightX = (timerBarObj.GetComponent<RectTransform>().rect.width / timerCloudXRatio) + textShakeX;
        timerCloudRightY = timerBarObj.GetComponent<RectTransform>().anchoredPosition.y + textShakeY;
        timerCloudScale = ((timerCloudScaleMax - timerCloudScaleMin) * currentFillAmount) + timerCloudScaleMin;
        timerCloudRightObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(timerCloudRightX, timerCloudRightY, 0);
        timerCloudRightObj.GetComponent<RectTransform>().localScale = new Vector3(timerCloudScale, timerCloudScale, 1);

        // set positoin of timer cloud shadow
        timerCloudRightShadowObj.transform.eulerAngles = timerCloudRightObj.transform.eulerAngles;
        timerCloudRightShadowObj.GetComponent<RectTransform>().localScale = new Vector3(timerCloudScale, timerCloudScale, 1);
        timerCloudRightShadowObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(
            timerCloudRightObj.GetComponent<RectTransform>().anchoredPosition.x + 2,
            timerCloudRightObj.GetComponent<RectTransform>().anchoredPosition.y - 2,
            0
        );

        
        // set color of timer clouds
        timerCloudRightObj.GetComponent<Image>().color = BlendColors(currentTimerBarColor, currentTimerBarColor, Color.white);

        // set position, scale, and text for time remaining
        timeRemainingIntObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(
            timerCloudRightObj.GetComponent<RectTransform>().anchoredPosition.x - 7,
            timerCloudRightObj.GetComponent<RectTransform>().anchoredPosition.y,
            0
        );
        timeRemainingDecObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(
            timerCloudRightObj.GetComponent<RectTransform>().anchoredPosition.x + 8,
            timerCloudRightObj.GetComponent<RectTransform>().anchoredPosition.y - 2,
            0
        );
        timeRemainingIntObj.GetComponent<RectTransform>().localScale = playerTurnTextObj.GetComponent<RectTransform>().localScale;
        timeRemainingDecObj.GetComponent<RectTransform>().localScale = playerTurnTextObj.GetComponent<RectTransform>().localScale;


        // get & set time remaining countdown string
        string timeRemainingStr = currentPlayerTimeRemaining.ToString();
        int timeRemainingStrLen = timeRemainingStr.Length;
        string timeRemainingIntStr = timeRemainingStr.Substring(0, Mathf.Min(2, timeRemainingStrLen));
        string timeRemainingDecStr = "0";
        if (timeRemainingStrLen >= 3) {
            timeRemainingDecStr = timeRemainingStr.Substring(2, 1);
        }
        timeRemainingIntObj.GetComponent<Text>().text = timeRemainingIntStr;
        timeRemainingDecObj.GetComponent<Text>().text = timeRemainingDecStr;
    }

    void FixedUpdate()
    {
        // decrease shake effect
        textShake = Mathf.Max(textShake - (22f * Time.deltaTime), 0);
        if (!gameEndObj.GetComponent<GameEnd>().gameEnd) {
            timerCloudRightObj.transform.Rotate(0, 0, -30 * Time.deltaTime);
        }
    }

    void UpdateText()
    {
        // update string saying current player's turn
        int currentPlayerTurn = camera.GetComponent<TurnSwitch>().currentPlayerTurn;
        int currentCharSelect = playerVarsObj.GetComponent<PlayerVars>().charSelect[currentPlayerTurn];

        string currentPlayerName = playerVarsObj.GetComponent<PlayerVars>().playerNames[currentCharSelect];
        playerTurnTextStr = currentPlayerName + "'S TURN!";
        playerTurnTextObj.GetComponent<Text>().text = playerTurnTextStr;
        turnTextScaleDest = 1;
    }

    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    public static Color ColorDamp(Color colorA, Color colorB, float lambda, float dt)
    {
        float r = Damp(colorA.r, colorB.r, lambda, dt);
        float g = Damp(colorA.g, colorB.g, lambda, dt);
        float b = Damp(colorA.b, colorB.b, lambda, dt);
        float a = Damp(colorA.a, colorB.a, lambda, dt);
        return new Color(r, g, b, a);
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
